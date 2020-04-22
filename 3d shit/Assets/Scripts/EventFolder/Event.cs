﻿using System;
using UnityEngine;

namespace EventCallbacks
{
    public abstract class Event
    {
        public string EventDescription;
        public GameObject UnitGameObject;
    }

    public class DebugEvent : Event
    {
        public String debugMessage;
    }

    public class UnitDeathEvent : Event
    {

    }

    public class SoundEvent : Event
    {
        //public Transform gameObjectTransform;
        public AudioClip sound;
        public AudioSource audioSource;
    }

    public class ParticleEvent : Event
    {
        //public Transform gameObjectTransform;
        public ParticleSystem particles;
    }
}