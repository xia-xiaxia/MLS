using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueMonitor<T> : MonoBehaviour
{
    private T value;
    public event Action<T> OnValueChanged;

    public T Value
    {
        get
        {
            return value;
        }
        set
        {
            if (!EqualityComparer<T>.Default.Equals(this.value, value))
            {
                this.value = value;
                OnValueChanged?.Invoke(this.value);
            }
        }
    }
}
