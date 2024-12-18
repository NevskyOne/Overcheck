﻿using TMPro;
using UnityEngine;

public class ToolBase : MonoBehaviour
{
    [Header("Script")]
    [SerializeField] private ToolName _toolName;
    [SerializeField] private int _price;
    [SerializeField] private int _index;
    [Header("Info")]
    [SerializeField][TextArea] private string _description;
    [SerializeField] private GameObject _infoObj;
    [SerializeField] private TMP_Text _infoText;
    [Header("Buttons")]
    [SerializeField] private GameObject BuyButton;
    [SerializeField] private GameObject SellButton;

    public string ToolName => _toolName.ToString();
    public int Price => _price;
    public int Index => _index;

    public void ShowInfo()
    {
        _infoText.text = _description;
        _infoObj.SetActive(true);
    }

    public void Buyed()
    {
        BuyButton.SetActive(false);
        SellButton.SetActive(true);
    }
    
    public void Sold()
    {
        BuyButton.SetActive(true);
        SellButton.SetActive(false);
    }
}