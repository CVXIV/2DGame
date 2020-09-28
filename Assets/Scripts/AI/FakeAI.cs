using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CVXIV {

    public enum AIState {
        Failure,
        Success,
        Continue,
        Abort
    }

    public static class FakeAI {
        public static SetActive SetActive(GameObject obj, bool isActive) {
            return new SetActive(obj, isActive);
        }

        public static Trigger Trigger(Animator animator, string paramName, bool set) {
            return new Trigger(animator, paramName, set);
        }

        public static SetBool SetBool(Animator animator, string paramName, bool value) {
            return new SetBool(animator, paramName, value);
        }

        public static Wait Wait(float seconds) {
            return new Wait(seconds);
        }

        public static Condition Condition(System.Func<bool> method) {
            return new Condition(method);
        }

        public static WaitForAnimatorState WaitForAnimatorState(Animator animator, string name, int layer = 0) {
            return new WaitForAnimatorState(animator, name, layer);
        }

        public static Action Call(System.Action method) {
            return new Action(method);
        }

        public static Action RunCoroutine(System.Func<IEnumerator<AIState>> coroutineFactory) {
            return new Action(coroutineFactory);
        }

        public static WaitForAnimatorSignal WaitForAnimatorSignal(Animator animator, string name, int layer = 0) {
            return new WaitForAnimatorSignal(animator, name, layer); 
        }

        public static Terminate Terminate() {
            return new Terminate(); 
        }

        public static Log Log(string msg) {
            return new Log(msg); 
        }

    }
    #region 基类
    public abstract class AINode {
        public abstract AIState Execute();
    }
    #endregion

    #region Unity框架相关
    public class SetActive : AINode {

        private readonly GameObject obj;
        private readonly bool isActive;

        public SetActive(GameObject obj, bool isActive) {
            this.obj = obj;
            this.isActive = isActive;
        }

        public override AIState Execute() {
            obj.SetActive(isActive);
            return AIState.Success;
        }
    }

    public class Trigger : AINode {

        private readonly Animator animator;
        private readonly string paramName;
        private readonly bool set;

        public Trigger(Animator animator, string paramName, bool set) {
            this.animator = animator;
            this.paramName = paramName;
            this.set = set;
        }

        public override AIState Execute() {
            if (set) {
                animator.SetTrigger(paramName);
            } else {
                animator.ResetTrigger(paramName);
            }
            return AIState.Success;
        }
    }

    public class SetBool : AINode {

        private readonly Animator animator;
        private readonly string paramName;
        private readonly bool value;

        public SetBool(Animator animator, string paramName, bool value) {
            this.animator = animator;
            this.paramName = paramName;
            this.value = value;
        }

        public override AIState Execute() {
            animator.SetBool(paramName, value);
            return AIState.Success;
        }
    }

    /// <summary>
    /// 暂停运行seconds秒
    /// </summary>
    public class Wait : AINode {
        private readonly float seconds;
        private float futureTime = -1;

        public Wait(float seconds) {
            this.seconds = seconds;
        }
        public override AIState Execute() {
            if (futureTime < 0) {
                futureTime = Time.time + seconds;
            }
            if (Time.time >= futureTime) {
                futureTime = -1;
                return AIState.Success;
            }
            return AIState.Continue;
        }
    }

    /// <summary>
    /// 调用方法，并且返回方法的返回值（bool）
    /// </summary>
    public class Condition : AINode {

        private readonly System.Func<bool> method;

        public Condition(System.Func<bool> method) {
            this.method = method;
        }

        public override AIState Execute() {
            return method() ? AIState.Success : AIState.Failure;
        }
    }

    /// <summary>
    /// 等待执行一个动画状态机
    /// </summary>
    public class WaitForAnimatorState : AINode {

        private readonly Animator animator;
        private readonly int layer;
        private readonly int id;

        public WaitForAnimatorState(Animator animator, string stateName, int layer = 0) {
            this.animator = animator;
            this.id = Animator.StringToHash(stateName);
            this.layer = layer;
            if (!animator.HasState(layer, id)) {
                Debug.LogError("The " + stateName + " does not exist");
            }
        }

        public override AIState Execute() {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(layer);
            if (state.shortNameHash == id || state.fullPathHash == id) {
                return AIState.Success;
            }
            return AIState.Continue;
        }
    }


    /// <summary>
    /// 等待一个动画状态机的信号
    /// </summary>
    public class WaitForAnimatorSignal : AINode {

        private readonly int id;
        internal bool isSet = false;

        public WaitForAnimatorSignal(Animator animator, string stateName, int layer = 0) {
            id = Animator.StringToHash(stateName);
            if (!animator.HasState(layer, id)) {
                Debug.LogError("The " + stateName + " does not exist");
            } else {
                SendSignal.Register(animator, stateName, this);
            }
        }

        public override AIState Execute() {
            if (!isSet) {
                return AIState.Continue;
            }
            isSet = false;
            return AIState.Success;
        }
    }

    public class Terminate : AINode {

        public override AIState Execute() {
            return AIState.Abort;
        }

    }

    public class Log : AINode {
        private readonly string msg;

        public Log(string msg) {
            this.msg = msg;
        }

        public override AIState Execute() {
            Debug.Log(msg);
            return AIState.Success;
        }
    }

    #endregion

    #region 角色状态控制
    /// <summary>
    /// 调用一个普通方法或者一个协程
    /// </summary>
    public class Action : AINode {

        private readonly System.Action method;
        private readonly System.Func<IEnumerator<AIState>> coroutineFactory;
        private IEnumerator<AIState> coroutine;

        public Action(System.Action method) {
            this.method = method;
        }

        public Action(System.Func<IEnumerator<AIState>> coroutineFactory) {
            this.coroutineFactory = coroutineFactory;
        }

        public override AIState Execute() {
            if (method != null) {
                method();
                return AIState.Success;
            } else {
                if (coroutine == null) {
                    coroutine = coroutineFactory();
                }
                if (!coroutine.MoveNext()) {
                    coroutine = null;
                    return AIState.Success;
                }
                AIState result = coroutine.Current;
                if (result != AIState.Continue) {
                    coroutine = null;
                }
                return result;
            }
        }
    }
    #endregion
}
