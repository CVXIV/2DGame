using BTAI;
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

        public static Action Call(System.Action method, System.Action cancelMethod = null) {
            return new Action(method, cancelMethod);
        }

        public static Action RunCoroutine(System.Func<IEnumerator<AIState>> coroutineFactory) {
            return new Action(coroutineFactory);
        }

        public static WaitForAnimatorSignal WaitForAnimatorSignal(Animator animator, string stateName, string signal, int layer = 0) {
            return new WaitForAnimatorSignal(animator, stateName, signal, layer);
        }

        public static Terminate Terminate() {
            return new Terminate();
        }

        public static Log Log(string msg) {
            return new Log(msg);
        }

        public static Sequence Sequence() {
            return new Sequence();
        }

        public static Selector Selector(bool shuffle) {
            return new Selector(shuffle);
        }

        public static ConditionalBranch If(System.Func<bool> method) {
            return new ConditionalBranch(method);
        }

        public static While While(System.Func<bool> method) {
            return new While(method);
        }

        public static Root Root() {
            return new Root();
        }

        public static Repeat Repeat(int totalCount) {
            return new Repeat(totalCount);
        }

        public static RandomSequence RandomSequence(int[] weights) {
            return new RandomSequence(weights);
        }

    }
    #region 基类
    public abstract class AINode {
        public abstract AIState Execute();
        public virtual void CancelExecute() { }
    }

    public abstract class Branch : AINode {

        protected int activeIndex = 0;
        protected List<AINode> elements = new List<AINode>();

        public virtual Branch OpenBranch(params AINode[] parms) {
            for (int i = 0; i < parms.Length; ++i) {
                elements.Add(parms[i]);
            }
            return this;
        }

        public virtual void ResetElements() {
            activeIndex = 0;
            foreach (AINode node in elements) {
                node.CancelExecute();
                if (node is Branch branch) {
                    branch.ResetElements();
                }
            }
        }

    }

    /// <summary>
    /// 整块执行，不管是否成功
    /// </summary>
    public abstract class Block : Branch {
        public override AIState Execute() {
            switch (elements[activeIndex].Execute()) {
                case AIState.Continue:
                    return AIState.Continue;
                default:
                    activeIndex++;
                    if (activeIndex == elements.Count) {
                        activeIndex = 0;
                        return AIState.Success;
                    }
                    return AIState.Continue;
            }
        }
    }

    #endregion

    #region 游戏逻辑框架
    public class Sequence : Branch {
        public override AIState Execute() {
            switch (elements[activeIndex].Execute()) {
                case AIState.Failure:
                    activeIndex = 0;
                    return AIState.Failure;
                case AIState.Success:
                    activeIndex++;
                    if (activeIndex == elements.Count) {
                        activeIndex = 0;
                        return AIState.Success;
                    }
                    return AIState.Continue;
                case AIState.Continue:
                    return AIState.Continue;
                case AIState.Abort:
                    activeIndex = 0;
                    return AIState.Abort;
            }
            throw new System.Exception("This should never happen, but clearly it has.Why??");
        }
    }

    /// <summary>
    /// 运行每个元素，直到返回Success；如果全部Failure，则返回Failure
    /// </summary>
    public class Selector : Branch {

        /// <summary>
        /// 是否打乱元素顺序
        /// </summary>
        /// <param name="shuffle"></param>
        public Selector(bool shuffle) {
            if (shuffle) {
                AINode temp;
                int swapIndex;
                for (int i = elements.Count - 1; i >= 0; --i) {
                    swapIndex = Random.Range(0, i + 1);
                    temp = elements[swapIndex];
                    elements[swapIndex] = elements[i];
                    elements[i] = temp;
                }
            }
        }

        public override AIState Execute() {
            switch (elements[activeIndex].Execute()) {
                case AIState.Failure:
                    activeIndex++;
                    if (activeIndex == elements.Count) {
                        activeIndex = 0;
                        return AIState.Failure;
                    }
                    return AIState.Continue;
                case AIState.Success:
                    activeIndex = 0;
                    return AIState.Success;
                case AIState.Continue:
                    return AIState.Continue;
                case AIState.Abort:
                    activeIndex = 0;
                    return AIState.Abort;
            }
            throw new System.Exception("This should never happen, but clearly it has.Why??");
        }
    }

    public class ConditionalBranch : Block {

        private readonly System.Func<bool> method;
        private bool isPassed = false;

        public ConditionalBranch(System.Func<bool> method) {
            this.method = method;
        }

        public override AIState Execute() {
            if (!isPassed) {
                isPassed = method();
            }
            if (isPassed) {
                AIState result = base.Execute();
                if (result != AIState.Continue) {
                    isPassed = false;
                }
                return result;
            }
            return AIState.Failure;
        }
    }

    /// <summary>
    /// 类似While循环那样执行元素
    /// </summary>
    public class While : Block {

        private readonly System.Func<bool> method;

        public While(System.Func<bool> method) {
            this.method = method;
        }

        public override AIState Execute() {
            // 如果方法返回false，则跳出循环并重置索引
            if (!method()) {
                ResetElements();
                return AIState.Failure;
            }
            base.Execute();
            return AIState.Continue;
        }
    }

    public class Root : Branch {
        private bool isTerminated = false;

        public override AIState Execute() {
            if (isTerminated) {
                return AIState.Abort;
            }
            switch (elements[activeIndex].Execute()) {
                case AIState.Continue:
                    return AIState.Continue;
                case AIState.Abort:
                    isTerminated = true;
                    return AIState.Abort;
                default:
                    activeIndex++;
                    if (activeIndex == elements.Count) {
                        activeIndex = 0;
                        return AIState.Success;
                    }
                    return AIState.Continue;
            }
        }
    }

    public class Repeat : Block {
        private readonly int totalCount;
        private int currentCount = 0;

        public Repeat(int totalCount) {
            this.totalCount = totalCount;
        }

        public override AIState Execute() {
            if (currentCount < totalCount) {
                AIState result = base.Execute();
                switch (result) {
                    case AIState.Success:
                        currentCount++;
                        if (currentCount == totalCount) {
                            currentCount = 0;
                            return AIState.Success;
                        }
                        return AIState.Continue;
                    default:
                        return AIState.Continue;
                }
            }
            return AIState.Success;
        }
    }

    public class RandomSequence : Block {

        private readonly int[] weights = null;
        private int[] cumulativeWeights = null;

        public RandomSequence(int[] weights) {
            activeIndex = -1;
            this.weights = weights;
        }

        /// <summary>
        /// 权重默认为1
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public override Branch OpenBranch(params AINode[] elements) {
            cumulativeWeights = new int[elements.Length];
            for (int i = 0; i < cumulativeWeights.Length; ++i) {
                int currentWeight;
                int preWeight = 0;
                if (weights == null || i >= weights.Length) {
                    currentWeight = 1;
                } else {
                    currentWeight = weights[i];
                }
                if (i > 0) {
                    preWeight = cumulativeWeights[i - 1];
                }
                cumulativeWeights[i] = currentWeight + preWeight;
            }
            return base.OpenBranch(elements);
        }

        public override AIState Execute() {
            if (activeIndex == -1) {
                PickOneElement();
            }
            AIState result = elements[activeIndex].Execute();
            switch (result) {
                case AIState.Continue:
                    return AIState.Continue;
                default:
                    PickOneElement();
                    return result;
            }
        }

        /// <summary>
        /// 根据权重的大小随机选择一个元素执行
        /// </summary>
        private void PickOneElement() {
            int choice = Random.Range(cumulativeWeights[0], cumulativeWeights[cumulativeWeights.Length - 1] + 1);
            for (int i = 0; i < cumulativeWeights.Length; ++i) {
                if (choice <= cumulativeWeights[i]) {
                    activeIndex = i;
                    break;
                }
            }
        }

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

        public override void CancelExecute() {
            futureTime = -1;
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

        public WaitForAnimatorSignal(Animator animator, string stateName, string signal, int layer = 0) {
            id = Animator.StringToHash(stateName);
            if (!animator.HasState(layer, id)) {
                Debug.LogError("The " + stateName + " does not exist");
            } else {
                SendSignal.Register(animator, signal, this);
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
        private readonly System.Action cancelMethod;
        private readonly System.Func<IEnumerator<AIState>> coroutineFactory;
        private IEnumerator<AIState> coroutine;

        public Action(System.Action method, System.Action cancelMethod) {
            this.method = method;
            this.cancelMethod = cancelMethod;
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

        public override void CancelExecute() {
            cancelMethod?.Invoke();
        }
    }
    #endregion
}
