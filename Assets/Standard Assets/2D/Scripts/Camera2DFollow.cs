using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class CameraBoundries
{
    public float top;
    public float bottom;
    public float left;
    public float right;
}

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        [SerializeField]
        public CameraBoundries[] limits;
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        


        private float m_OffsetZ;
        private Vector3 m_OffsetY;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        // Use this for initialization
        private void Start()
        {
            m_OffsetZ = (transform.position - target.position).z;
            m_LastTargetPosition = target.position + m_OffsetY;
            transform.parent = null;
        }


        // Update is called once per frame
        private void Update()
        {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }
            
            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = new Vector3(
                    Mathf.Clamp(newPos.x, limits[0].left, limits[0].right),
                    Mathf.Clamp(newPos.y, limits[0].bottom, limits[0].top),
                    newPos.z
                );

            //transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for(int counter = 0; counter < limits.Length; counter++)
            {
                Gizmos.DrawLine(new Vector2(limits[counter].left, limits[counter].top), new Vector2(limits[counter].right, limits[counter].top));
                Gizmos.DrawLine(new Vector2(limits[counter].left, limits[counter].top), new Vector2(limits[counter].left, limits[counter].bottom));
                Gizmos.DrawLine(new Vector2(limits[counter].right, limits[counter].bottom), new Vector2(limits[counter].left, limits[counter].bottom));
                Gizmos.DrawLine(new Vector2(limits[counter].right, limits[counter].top), new Vector2(limits[counter].right, limits[counter].bottom));
            }
            
        }
    }
}
