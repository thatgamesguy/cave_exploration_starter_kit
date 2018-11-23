using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CaveExploration
{
	public class Events
	{
		private static Events _instance;
		public static Events instance {
			get {
				if (_instance == null) {
					_instance = new Events ();
				}

				return _instance;
			}
		}

		public delegate void EventDelegate<T> (T e) where T: GameEvent;
		private delegate void EventDelegate (GameEvent e);

		private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate> ();
		private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate> ();

		public void AddListener<T> (EventDelegate<T> del) where T : GameEvent
		{

			EventDelegate internalDelegate = (e) => del ((T)e);
			if (delegateLookup.ContainsKey (del) && delegateLookup [del] == internalDelegate) {
				return;
			}

			// Create non-generic delegate.

			delegateLookup [del] = internalDelegate;

			EventDelegate tempDel;
			if (delegates.TryGetValue (typeof(T), out tempDel)) {
				delegates [typeof(T)] = tempDel += internalDelegate; 
			} else {
				delegates [typeof(T)] = internalDelegate;
			}
		}

		public void RemoveListener<T> (EventDelegate<T> del) where T : GameEvent
		{
			EventDelegate internalDelegate;
			if (delegateLookup.TryGetValue (del, out internalDelegate)) {
				EventDelegate tempDel;
				if (delegates.TryGetValue (typeof(T), out tempDel)) {
					tempDel -= internalDelegate;
					if (tempDel == null) {
						delegates.Remove (typeof(T));
					} else {
						delegates [typeof(T)] = tempDel;
					}
				}
				
				delegateLookup.Remove (del);
			}
		}

		public void Raise (GameEvent e)
		{
			EventDelegate del;
			if (delegates.TryGetValue (e.GetType (), out del)) {
				del.Invoke (e);
			}
		}

	}
}
