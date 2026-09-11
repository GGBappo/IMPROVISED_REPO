using System;
using UnityEditor.UI;
using UnityEngine;

public class BeerPongCup : MonoBehaviour
{
    [SerializeField] private int ID;
    [SerializeField] private Transform originalPosition;

    void Start()
    {
        originalPosition = this.transform;
    }

    void OnEnable() 
    {
        
    }

    void OnDisable()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        GameEvents.PingPongBallEnteredCup(ID, this.transform);
    }

    public int GetID()
    {
        return ID;
    }
}
