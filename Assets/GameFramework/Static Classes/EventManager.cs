using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace GF
{
    /// <summary>
    /// Manages All Events in the game.
    /// </summary>
    internal class EventManager
    {
        private static EventManager _instance = null;
        internal static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventManager();
                }
                return _instance;
            }
        }
        private readonly Dictionary<Type, EventListener> eventListeners = new Dictionary<Type, EventListener>();
        private readonly Queue<GameEvent> queueEvents = new Queue<GameEvent>();
        private EventListener currentEvent = null;
        /// <summary>
        /// Add particular Evnet to event dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventHandler"></param>
        internal void AddListener<T>(EventListener.EventHandler<T> eventHandler) where T : GameEvent
        {
            if (!eventListeners.TryGetValue(typeof(T), out EventListener invoker))
            {
                invoker = new EventListener();
                eventListeners.Add(typeof(T), invoker);
            }
            invoker.eventHandler += (e) => eventHandler((T)e);
        }
        internal void QueueEvent(GameEvent ev)
        {
            queueEvents.Enqueue(ev);
        }
        internal void Update()
        {
            if(queueEvents.Count>0 && IsCurrentEventCompleted())
            {
                GameEvent evt = queueEvents.Dequeue();
                if (eventListeners.TryGetValue(evt.GetType(), out EventListener invoker))
                {
                    currentEvent = invoker;
                    invoker.Invoke(evt);
                }
            }
        }

        private bool IsCurrentEventCompleted()
        {
            if (currentEvent == null) return true;
            return currentEvent.IsDone();
        }

        /// <summary>
        /// Remove particular Evnet from event dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventHandler"></param>
        internal void RemoveListener<T>(EventListener.EventHandler<T> eventHandler) where T : GameEvent
        {
            if (eventListeners.TryGetValue(typeof(T), out EventListener invoker))
            {
                invoker.eventHandler -= (e) => eventHandler((T)e);
                eventListeners.Remove(typeof(T));
            }
        }
        /// <summary>
        /// Check Evnet in Event dictionry.
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        internal bool HasListener(Type eventType)
        {
            return eventListeners.ContainsKey(eventType);
        }

        /// <summary>
        /// Triger particular event.
        /// </summary>
        /// <param name="evt"></param>
        internal void TriggerEvent(GameEvent evt)
        {
            if (eventListeners.TryGetValue(evt.GetType(), out EventListener invoker))
            {
                invoker.Invoke(evt);
            }
        }
       
        /// <summary>
        /// Release all events.
        /// </summary>
        internal void ReleaseEvents()
        {
            foreach (EventListener value in eventListeners.Values)
            {
                value.Clear();
            }
            eventListeners.Clear();
        }
    }
    /// <summary>
    /// Hold Evnet in it
    /// </summary>
    public class EventListener
    {
        public delegate void EventHandler(GameEvent ge);
        public delegate void EventHandler<T>(T e) where T : GameEvent;

        public EventHandler eventHandler;
        private GameEvent gameEvent;
        public void Invoke(GameEvent ge)
        {
            gameEvent = ge;
            eventHandler?.Invoke(ge);
        }

        public void Clear()
        {
            eventHandler = null;
        }
        public bool IsDone()
        {
            return gameEvent.IsDone();
        }
    }
    /// <summary>
    /// Super class for event classes.
    /// Any class can inherit and become class event driver.
    /// </summary>
    public class GameEvent
    {
        private bool isDone=false;
        public bool IsDone()
        {
            return isDone;
        }
        public void SetIsDone(bool status)
        {
            isDone = status;
        }
    }
}
