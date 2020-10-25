using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CVXIV {

    [RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour {

        protected static PlayerController _instance;

        public static PlayerController Instance {
            get {
                return _instance;
            }
        }


        private bool isFlip;
        private Rigidbody2D rigid;
        private Animator animator;
        private Damage damage;
        private BeDamage beDamage;
        private Collider2D box;
        private int bulletLayerMask;
        private PassPlatform passPlatform;
        private readonly List<Collider2D> contacts = new List<Collider2D>();
        private float xScale, yScale;
        private bool isPause = false;
        private readonly RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[4];

        public AttackRange normalAttackCheck;
        public float maxSpeedX = 7;
        public float maxSpeedY = 15;
        // 子弹生成位置
        public Transform bulletPos;
        public GameObject bullet;
        private float timeY = 0f;
        public float holdingGunDuration = 1.5f;
        // 水平加速度
        private readonly float groundAcceleration = 80f;
        // 水平减速度
        private readonly float groundDeceleration = 80f;
        // 射击攻击间隔
        private readonly float normalAttackGap = 0.5f;
        private readonly float skillAttackGap = 0.8f;
        private bool isReadyNormalAttack = true;
        private bool isReadySkillAttack = true;

        private Vector2 currentVelocity;
        private bool isOnGround = true;

        // 动画参数
        protected readonly int hashHorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
        protected readonly int hashVerticalSpeed = Animator.StringToHash("VerticalSpeed");
        protected readonly int hashLocomotionSpeed = Animator.StringToHash("LocomotionSpeed");
        protected readonly int hashCrouching = Animator.StringToHash("Crouching");
        protected readonly int hashGround = Animator.StringToHash("grounded");
        protected readonly int hashPushing = Animator.StringToHash("Pushing");
        protected readonly int hashSkillAttack = Animator.StringToHash("skillAttack");
        protected readonly int hashNormalAttack = Animator.StringToHash("normalAttack");
        protected readonly int hashOnHurt = Animator.StringToHash("onHurt");
        protected readonly int hashInvincible = Animator.StringToHash("Invincible");
        protected readonly int hashDead = Animator.StringToHash("Dead");
        // 重生点
        private CheckPoint checkPoint;
        private Vector3 initPos;

        private void Awake() {
            if (_instance == null) {
                _instance = this;
            } else {
                throw new UnityException("There cannot be more than one PlayerController script.  The instances are " + _instance.name + " and " + name + ".");
            }


            xScale = Mathf.Abs(transform.localScale.x);
            yScale = Mathf.Abs(transform.localScale.y);
            InitBeDamage();
            damage = GetComponent<Damage>();
            box = GetComponent<BoxCollider2D>();
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            SceneLinkedSMB<PlayerController>.Initialise(animator, this);
            bulletLayerMask = LayerMask.GetMask(ConstantVar.GroundLayerName);
            isFlip = transform.localScale.x == -xScale;
            currentVelocity = rigid.velocity;
            initPos = transform.position;
        }

        private void FixedUpdate() {
            rigid.velocity = currentVelocity;
            animator.SetFloat(hashHorizontalSpeed, rigid.velocity.x);
            animator.SetFloat(hashVerticalSpeed, rigid.velocity.y);
            CheckGround();
        }

        private void Update() {
            if (PlayerInput.Instance.Pause.Down) {
                if (!isPause) {
                    if (ScreenFader.IsFading) {
                        return;
                    }
                    PlayerInput.Instance.ReleaseControl(false);
                    PlayerInput.Instance.Pause.GainControl();
                    isPause = true;
                    Time.timeScale = 0;
                    SceneManager.LoadSceneAsync(ConstantVar.PauseMenuName, LoadSceneMode.Additive);
                } else {
                    Unpause();
                }
            }
        }

        public void Unpause() {
            if (Time.timeScale != 0) {
                return;
            }
            StartCoroutine(UnpauseCoRoutine());
        }

        private IEnumerator UnpauseCoRoutine() {
            yield return SceneManager.UnloadSceneAsync(ConstantVar.PauseMenuName);
            Time.timeScale = 1;
            yield return Yields._FixedUpdate;
            PlayerInput.Instance.GainControl();
            isPause = false;
        }

        private void CheckGround() {
            Vector3 left = new Vector3(box.bounds.center.x - box.bounds.extents.x, box.bounds.center.y - box.bounds.extents.y, box.bounds.center.z);
            Vector3 right = new Vector3(box.bounds.center.x + box.bounds.extents.x, box.bounds.center.y - box.bounds.extents.y, box.bounds.center.z);
            // 发射4条射线确认角色是否处于地面
            RaycastHit2D hit1 = Physics2D.Raycast(left, Vector3.down, box.bounds.extents.y * 0.5f, 1 << ConstantVar.groundLayer);
            RaycastHit2D hit2 = Physics2D.Raycast(right, Vector3.down, box.bounds.extents.y * 0.5f, 1 << ConstantVar.groundLayer);
            RaycastHit2D hit3 = Physics2D.Raycast(left, Vector3.up, box.bounds.extents.y * 0.5f, 1 << ConstantVar.groundLayer);
            RaycastHit2D hit4 = Physics2D.Raycast(right, Vector3.up, box.bounds.extents.y * 0.5f, 1 << ConstantVar.groundLayer);
            isOnGround = hit1 || hit2 || hit3 || hit4;

            Collider2D transformCollider = hit1.collider != null ? hit1.collider : (hit2.collider != null ? hit2.collider : (hit3.collider != null ? hit3.collider : hit4.collider));
            // 如果是空中平台，则需要判断是否触碰了上边的碰撞线
            if (isOnGround && transformCollider.CompareTag(ConstantVar.SkyGroundTag)) {
                passPlatform = transformCollider.transform.GetComponent<PassPlatform>();
                float transform_y = transformCollider.bounds.center.y + transformCollider.bounds.extents.y;
                if (passPlatform != null) {
                    if (passPlatform.ReadyToGround) {
                        isOnGround = true;
                    } else {
                        isOnGround = passPlatform.ReadyToGround = left.y - transform_y >= 0;
                    }
                }
            } else {
                if (passPlatform != null) {
                    passPlatform.ReadyToGround = false;
                    passPlatform = null;
                }
            }
        }

        public void CheckCrouching() {
            animator.SetBool(hashCrouching, PlayerInput.Instance.Vertical.Value < 0);
        }

        public void SetLocomotionSpeed() {
            animator.SetFloat(hashLocomotionSpeed, Mathf.Approximately(currentVelocity.x, 0) ? 1 : currentVelocity.x / 3);
        }

        public void GroundedHorizontalMovement(bool useInput) {
            // 最终速度
            float desiredSpeed = useInput ? PlayerInput.Instance.Horizontal.Value * maxSpeedX : 0f;
            float acceleration = useInput && PlayerInput.Instance.Horizontal.ReceivingInput ? groundAcceleration : groundDeceleration;
            currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, desiredSpeed, acceleration * Time.deltaTime);
            currentVelocity.y = rigid.velocity.y;
        }

        public void GroundedCheckJump() {
            if (PlayerInput.Instance.Jump.Down) {
                timeY = 0;
                currentVelocity.y = maxSpeedY;
            }
        }

        public void JumpingUpdateJump() {
            timeY += Time.deltaTime;
            if (PlayerInput.Instance.Jump.Up || PlayerInput.Instance.Jump.Down) {
                timeY = 0.22f;
            } else if (PlayerInput.Instance.Jump.Held && timeY < 0.2f) {
                currentVelocity.y = maxSpeedY;
                isOnGround = false;
            }
        }

        public void CheckFacing() {
            bool left = PlayerInput.Instance.Horizontal.Value < 0;
            bool right = PlayerInput.Instance.Horizontal.Value > 0;
            if (left) {
                transform.localScale = new Vector2(-xScale, yScale);
                isFlip = true;
            } else if (right) {
                transform.localScale = new Vector2(xScale, yScale);
                isFlip = false;
            }
        }

        public void CheckIsOnGround() {
            animator.SetBool(hashGround, isOnGround);
        }

        public void CrouchingCheckDownJump() {
            if (passPlatform != null && PlayerInput.Instance.Jump.Down && PlayerInput.Instance.Vertical.Value < -float.Epsilon) {
                passPlatform.Pass(this.gameObject);
            }
        }

        public void CheckIsPush() {
            if (PlayerInput.Instance.Horizontal.ReceivingInput) {
                int count = Physics2D.BoxCastNonAlloc(box.bounds.center, 2 * box.bounds.extents, 0, isFlip ? Vector2.left : Vector2.right, raycastHit2Ds, 0.1f, LayerMask.GetMask(ConstantVar.GroundLayerName));
                if (count > 0 && raycastHit2Ds[0].collider.GetComponent<Pushable>() != null) {
                    animator.SetBool(hashPushing, true);
                    return;
                }
            }
            animator.SetBool(hashPushing, false);
        }

        public void SetIsReadyNormalAttack(bool isReadyNormalAttack) {
            this.isReadyNormalAttack = isReadyNormalAttack;
            if (!isReadyNormalAttack) {
                Invoke(nameof(ResetNormalAttack), normalAttackGap);
            }
        }

        public void CheckNormalAttack() {
            if (isReadyNormalAttack && PlayerInput.Instance.NormalAttack.Held) {
                animator.SetTrigger(hashNormalAttack);
            }
        }

        private void ResetNormalAttack() {
            isReadyNormalAttack = true;
        }


        public void CheckSkillAttack() {
            if (isReadySkillAttack && PlayerInput.Instance.SkillAttack.Held) {
                animator.SetTrigger(hashSkillAttack);
                isReadySkillAttack = false;
                Invoke(nameof(ResetSkillAttack), skillAttackGap);
            }
        }

        private void ResetSkillAttack() {
            isReadySkillAttack = true;
        }

        public void MakeBullet() {
            // 首先检测是否有障碍物
            Vector2 testPosition = transform.position;
            testPosition.y = bulletPos.position.y;
            Vector2 direction = (Vector2)bulletPos.position - testPosition;
            float distance = direction.magnitude;
            direction.Normalize();
            if (Physics2D.Raycast(testPosition, direction, distance, bulletLayerMask)) {
                return;
            }
            Bullet newBullet = Instantiate(bullet).GetComponent<Bullet>();
            newBullet.SetPosition(bulletPos.transform.position);
            newBullet.SetSpeed(!isFlip);
        }


        public void AttackDamage() {
            damage.Attack(normalAttackCheck.GetDamageObjs().ToArray());
        }

        #region 受伤，复活
        private void InitBeDamage() {
            beDamage = GetComponent<PlayerBeDamage>();
            beDamage.onHurt += OnHurt;
            beDamage.onDead += OnDead;
        }

        private void OnHurt(DamageType damageType, int damageNum) {
            animator.SetTrigger(hashOnHurt);
            switch (damageType) {
                case DamageType.Normal:
                    SetInvincible(1);
                    break;
                case DamageType.Dead:
                    // 播放死亡动画，重置位置
                    StartCoroutine(DieRespawnCoroutine(false, true));
                    break;
            }

        }

        private void OnDead(int damageNum) {
            animator.SetTrigger(hashOnHurt);
            StartCoroutine(DieRespawnCoroutine(true, false));
        }

        /// <summary>
        /// 使得角色无敌一段时间
        /// </summary>
        /// <param name="time"></param>
        private void SetInvincible(float time) {
            animator.SetBool(hashInvincible, true);
            beDamage.Disable();
            // 避免和Enemy碰撞
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ConstantVar.EnemyLayer), LayerMask.NameToLayer(ConstantVar.PlayLayer), true);
            Invoke(nameof(RestoreEnable), time);
        }

        private void RestoreEnable() {
            beDamage.Enable();
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ConstantVar.EnemyLayer), LayerMask.NameToLayer(ConstantVar.PlayLayer), false);
            animator.SetBool(hashInvincible, false);
        }

        private void PrepareDead() {
            PlayerInput.Instance.ReleaseControl();
            beDamage.Disable();
            animator.SetBool(hashDead, true);
        }

        /// <summary>
        /// 角色受到重置位置攻击时调用
        /// </summary>
        private IEnumerator DieRespawnCoroutine(bool resetHealth, bool useCheckPoint) {
            PrepareDead();
            yield return Yields.GetWaitForSeconds(1f); // 等待1秒
            yield return StartCoroutine(ScreenFader.FadeSceneIn(useCheckPoint ? ScreenFader.FadeType.Black : ScreenFader.FadeType.GameOver));
            // 如果是角色血量耗尽，则进入游戏结束界面且多等待2秒以展示该界面
            if (!useCheckPoint) {
                yield return Yields.GetWaitForSeconds(2f);
            }
            Respawn(resetHealth, useCheckPoint);
            yield return Yields._endOfFrame;
            yield return StartCoroutine(ScreenFader.FadeSceneOut());
            PlayerInput.Instance.GainControl();
            SetInvincible(2);
        }

        /// <summary>
        /// 重置角色位置
        /// </summary>
        /// <param name="resetHealth"></param>
        /// <param name="useCheckpoint"></param>
        private void Respawn(bool resetHealth, bool useCheckpoint) {
            if (resetHealth) {
                beDamage.ResetHealth();
            }
            if (useCheckpoint && checkPoint != null) {
                SetFacing(checkPoint.playerFacingRight);
                GameObjectTeleporter.Teleport(gameObject, checkPoint.transform.position);
            } else {
                GameObjectTeleporter.Teleport(gameObject, initPos);
            }
            animator.SetBool(hashDead, false);
        }


        public void CheckPoint(CheckPoint checkPoint) {
            this.checkPoint = checkPoint;
        }

        public void SetFacing(bool isRight) {
            isFlip = !isRight;
            transform.localScale = isFlip ? new Vector2(-xScale, yScale) : new Vector2(xScale, yScale);
        }

        public void SetKinematic(bool isKinematic) {
            currentVelocity = rigid.velocity = Vector2.zero;
            rigid.bodyType = isKinematic ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
        }

        #endregion
    }
}

