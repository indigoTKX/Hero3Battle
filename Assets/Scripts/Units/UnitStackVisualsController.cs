using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitStackVisualsController : MonoBehaviour
{
    [SerializeField] private Transform _model; 
    [SerializeField] private TMP_Text _unitCounter;
    [SerializeField] private TMP_Text _unitName;
    
    [SerializeField] private Vector3 _selectedOffset;

    private UnitStackController _unitController;
    
    private Vector3 _defaultModelPosition;
    private bool _isSelected = false;
    
    private void Awake()
    {
        _unitController = GetComponent<UnitStackController>();
        _unitController.OnInitialized += Initialize;
        _unitController.OnSelected += SetSelected;
        // _unitController.OnMoved +=
        _unitController.OnDamaged += UpdateCounter;
    }

    private void OnDestroy()
    {
        _unitController.OnInitialized -= Initialize;
        _unitController.OnSelected -= SetSelected;
        _unitController.OnDamaged -= UpdateCounter;
    }
    
    private void Initialize(int initialStackCount, UnitData unitData)
    {
        _unitCounter.text = initialStackCount.ToString();
        _unitName.text = unitData.Name;

        _defaultModelPosition = _model.localPosition;
        // if (!unitData.modelPrefab) return;
    }

    private void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        
        if (_isSelected)
        {
            _model.localPosition += _selectedOffset;
        }
        else
        {
            _model.localPosition = _defaultModelPosition;
        }
    }

    private void UpdateCounter()
    {
        _unitCounter.text = _unitController.GetCurrentStackCount().ToString();
    }
}
