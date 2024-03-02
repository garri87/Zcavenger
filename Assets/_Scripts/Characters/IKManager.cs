using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.XR;

public class IKManager : MonoBehaviour
{
    public bool isIKActive;

    public enum PlayerType
    {
        Player,
        Enemy,
    }
    public PlayerType playerType;

    public Animator _animator;

    public string upperBodyLayerName = "UpperBody";
    public bool upperMaskActive;
    private bool[] upperMaskConditions;

    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform leftElbowTarget;
    public Transform rightElbowTarget;
    public Transform spineTarget;


    public Transform agentHead;

    private PlayerController _playerController;
    private AgentController _agentController;
    private Inventory _inventory;
    public Item weaponItem;

    private Transform muzzleTransform;

    public Vector3 targetPosition;

    #region Recoil
    public bool recoilActive;
    public AnimationCurve bulletRecoilCurve;
    public float recoilMaxRotation;
    public float recoilDuration;
    public float recoilTimer;
    #endregion

    private float leftHandIKWeight;
    private float rightHandIKWeight;
    private float leftHandRotWeight;
    private float rightHandRotWeight;
    private float leftElbowIKWeight;
    private float rightElbowIKWeight;

    public float distanceToObject;
    public float verticalOffset;

    [HideInInspector] public static bool pushingObject; //TODO: Implementar detección cuando esta empujando un objeto

    private RaycastHit frontHit, backHit, leftHit, rightHit;

    public LayerMask layer, footIkLayer;


    [Range(0, 1)] public float weight = 1f;
    [Range(0, 1)] public float upperBodyLayerWeight;

    private float distanceToGround;

    void Awake()
    {
        switch (playerType)
        {
            case PlayerType.Player:
                _playerController = GetComponent<PlayerController>();
                _inventory = GetComponent<Inventory>();
                break;

            case PlayerType.Enemy:
                _agentController = GetComponent<AgentController>();
                break;
        }

        _animator = GetComponent<Animator>();
        spineTarget = _animator.GetBoneTransform(HumanBodyBones.Spine);
    }

    void FixedUpdate()
    {

        SetLayerWeight();

    }

    
    private void Update()
    {
        
        
        switch (playerType)
        {
            case PlayerType.Player:

                upperMaskActive = ActiveUpperMask();
                
                SetLayerWeight();

                upperBodyLayerWeight = _animator.GetLayerWeight(1);
                _animator.SetFloat("LayerWeight", upperBodyLayerWeight);
                if (_inventory.drawnWeaponItem)
                {
                    weaponItem = _inventory.drawnWeaponItem;
                    recoilMaxRotation = weaponItem.recoilMaxRotation;
                    recoilDuration = weaponItem.recoilDuration;
                }


                if (_playerController.isAiming)
                {
                    weight = 0.4f;
                }
                else
                {
                    weight = 0;
                }
                break;

            case PlayerType.Enemy:

                SetLayerWeight();
                if (_agentController._enemyFov.targetInSight || _agentController._enemyFov.targetInRange)
                {
                    agentHead.LookAt(targetPosition + Vector3.up);
                    weight = 1;
                }
                else
                {
                    //weight = 0;
                }
                break;
        }

    }

    private void LateUpdate()
    {
        switch (playerType)
        {
            case PlayerType.Player:
                if (_inventory.drawnWeaponItem && _playerController.isAiming)
                {
                    if (!_playerController.isDrinking
                        || !_playerController.isBandaging
                        || !_playerController.isEating)
                    {
                        if (recoilActive)
                        {
                            RecoilAnimation();
                        }
                    }
                }
                break;
            case PlayerType.Enemy:

                break;
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (isIKActive)
        {
            switch (playerType)
            {
                case PlayerType.Player:

                    IKLimbPlacement(AvatarIKGoal.LeftFoot, AvatarIKGoal.RightFoot, "IKLeftFootWeight", "IKRightFootWeight", transform.TransformDirection(Vector3.down), distanceToGround);
                    /*
                    if (!_playerController.weaponDrawn)
                    {
                        _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandIKWeight);
                        _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotWeight);
                        _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                        _animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
                        _animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowTarget.position);
                        _animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftElbowIKWeight);

                        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandIKWeight);
                        _animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightElbowIKWeight);
                        _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandRotWeight);
                        _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                        _animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
                        _animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowTarget.position);
                    }*/
                    break;

                case PlayerType.Enemy:

                    break;
            }


        }
    }

    public void SetLayerWeight()
    {
        switch (playerType)
        {
            case PlayerType.Player:

                if (!_playerController._healthManager.IsDead)
                { //activates the upper boy layer of the animator if one of the actions is being played 
                    if (upperMaskActive)
                    {
                        if (!_playerController.climbingLadder ||
                            !_inventory.drawnWeaponItem.attacking ||
                            !_playerController.isBlocking)
                        {
                            _animator.SetLayerWeight(_animator.GetLayerIndex(upperBodyLayerName), 1);
                        }
                        else
                        {
                            _animator.SetLayerWeight(_animator.GetLayerIndex(upperBodyLayerName), 0);
                        }
                    }

                    else
                    {
                        _animator.SetLayerWeight(_animator.GetLayerIndex(upperBodyLayerName), 0);
                    }
                }
                else
                {
                    _animator.SetLayerWeight(_animator.GetLayerIndex(upperBodyLayerName), 0);
                }
                break;

            case PlayerType.Enemy:

                if (_agentController.attacking ||
                    _agentController.playerCatch ||
                    /*_agentController._enemyFov.targetInSight||*/ _agentController.hisHit)
                {
                    _animator.SetLayerWeight(_animator.GetLayerIndex(upperBodyLayerName), 1);
                }
                else
                {
                    _animator.SetLayerWeight(_animator.GetLayerIndex(upperBodyLayerName), 0);
                }


                break;

        }

    }

    private void IKLimbPlacement(AvatarIKGoal leftLimb, AvatarIKGoal rightLimb,
        string ikLeftWeight, string iKRightWeight, Vector3 rayDirection, float rayDistance)
    {
        //GET LIMB WEIGHT FROM ANIMATOR CURVES
        _animator.SetIKPositionWeight(leftLimb, _animator.GetFloat(ikLeftWeight));
        _animator.SetIKRotationWeight(leftLimb, _animator.GetFloat(ikLeftWeight));
        _animator.SetIKPositionWeight(rightLimb, _animator.GetFloat(iKRightWeight));
        _animator.SetIKRotationWeight(rightLimb, _animator.GetFloat(iKRightWeight));

        RaycastHit hit1; // LEFT FOOT RAYCAST

        Ray leftFootRay = new Ray(_animator.GetIKPosition(leftLimb), rayDirection);
        if (Physics.Raycast(leftFootRay, out hit1, rayDistance, footIkLayer.value))
        {
            if (hit1.transform.tag == "Ground")
            {
                Vector3 leftFootPosition = hit1.point;
                leftFootPosition.y += rayDistance;
                Vector3 leftFoot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                _animator.SetIKPosition(leftLimb, new Vector3(leftFoot.x, leftFoot.y, leftFoot.z));
                _animator.SetIKRotation(leftLimb, Quaternion.FromToRotation(Vector3.up, hit1.normal) * transform.rotation);
            }
        }

        RaycastHit hit2; //RIGHT FOOT RAYCAST

        Ray ray2 = new Ray(_animator.GetIKPosition(rightLimb), rayDirection);

        if (Physics.Raycast(ray2, out hit2, rayDistance, footIkLayer.value))
        {
            if (hit2.transform.tag == "Ground")
            {
                Vector3 rightFootPosition = hit2.point;
                rightFootPosition.y += rayDistance;
                Vector3 rightFoot = _animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
                _animator.SetIKPosition(rightLimb, new Vector3(rightFoot.x, rightFoot.y, rightFoot.z));
                _animator.SetIKRotation(rightLimb, Quaternion.FromToRotation(Vector3.up, hit2.normal) * transform.rotation);
            }
        }
        Debug.DrawRay(_animator.GetIKPosition(leftLimb), rayDirection * distanceToGround, Color.green);
        Debug.DrawRay(_animator.GetIKPosition(rightLimb), rayDirection * distanceToGround, Color.green);
    }



    private void RecoilAnimation()
    {
        if (recoilTimer < 0)
        {
            return;
        }

        float curveTime = (Time.time - recoilTimer) / recoilDuration;
        if (curveTime > 1f)
        {
            recoilTimer = -1;
        }
        else
        {
            leftElbowTarget.Rotate(Vector3.up, bulletRecoilCurve.Evaluate(curveTime) * recoilMaxRotation, Space.Self);
            rightElbowTarget.Rotate(Vector3.up, bulletRecoilCurve.Evaluate(curveTime) * recoilMaxRotation, Space.Self);
            spineTarget.Rotate(Vector3.left, bulletRecoilCurve.Evaluate(curveTime) * recoilMaxRotation / 2, Space.Self);
        }
    }
    
    public bool ActiveUpperMask()
    {
        upperMaskConditions = new bool[]{
            _inventory.drawWeapon,
            _inventory.holsterWeapon ,
            _playerController.isAiming ,
            _playerController.isReloadingWeapon ,
            _playerController._healthManager.isBleeding ,
            _playerController.isBandaging ,
            _playerController.isDrinking ,
            _playerController.isEating ,
            _playerController.grabbingItem
        };

        bool active = false;

        for (int i = 0; i < upperMaskConditions.Length; i++)
        {
            if (upperMaskConditions[i] == true)
            {
                active = true;
                break;
            }
        }
        return active;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up * verticalOffset, transform.position + Vector3.up * verticalOffset + transform.forward * distanceToObject);
        Gizmos.DrawLine(transform.position + Vector3.up * verticalOffset, transform.position + Vector3.up * verticalOffset - transform.forward * distanceToObject);



        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.up * verticalOffset,
            transform.position + Vector3.up * verticalOffset + transform.right * distanceToObject);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position + Vector3.up * verticalOffset,
            transform.position + Vector3.up * verticalOffset - transform.right * distanceToObject);
    }
}
