using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ball
{
    public class BallMovement : MonoBehaviour
    {
        [SerializeField] private float jumpForce;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log(other.gameObject.name);
            ApplyForce();   
        }
        
        
        private void ApplyForce()
        {
            _rb.velocity = Vector3.zero;
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
    }
}