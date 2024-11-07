using System;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class PartMachine<T>
{


    public abstract class Part
    {

        protected readonly T _Context;

        public Part(T Context)
        {
            _Context = Context;
        }

        public virtual void Update()
        {

        }

    }

    private Dictionary<Type, Part> _Parts = new();


    public PartMachine<T> With(Part part)
    {
        _Parts.Add(part.GetType(), part);

        return this;
    }

    public void Update()
    {
        foreach (var part in _Parts.Values)
        {
            part.Update();
        }
    }

}