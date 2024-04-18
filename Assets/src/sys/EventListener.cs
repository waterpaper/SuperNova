using System;
using System.Collections.Generic;

namespace Supernova.Unity
{
    public interface IEvent { }

    /// <summary>
    /// �̺�Ʈ ������ �����ϱ� ���� Ŭ�����Դϴ�.
    /// �������� �̺�Ʈ�� ȣ���Ҷ� ����մϴ�.
    /// </summary>
    public class EventListeners
    {
        private readonly Dictionary<Type, List<Action<IEvent>>> listeners = new Dictionary<Type, List<Action<IEvent>>>();
        private readonly Dictionary<object, Action<IEvent>> actions = new Dictionary<object, Action<IEvent>>();

        public void On<T>(Action<T> action) where T : class, IEvent
        {
            var t = typeof(T);
            List<Action<IEvent>> events;
            if (!listeners.TryGetValue(t, out events))
            {
                events = new List<Action<IEvent>>();
                listeners.Add(t, events);
            }

            Action<IEvent> n = (IEvent e) => { action(e as T); };
            actions.Add(action, n);
            events.Add(n);
        }

        public void Off<T>(Action<T> action) where T : class, IEvent
        {
            var t = typeof(T);
            List<Action<IEvent>> events;
            if (!listeners.TryGetValue(t, out events))
            {
                throw new Exception();
            }

            Action<IEvent> n;
            if (!actions.TryGetValue(action, out n))
            {
                throw new Exception();
            }

            actions.Remove(action);

            if (!events.Remove(n))
            {
                throw new Exception();
            }
        }

        public void Emit<T>(T e) where T : class, IEvent
        {
            var t = typeof(T);
            List<Action<IEvent>> events;
            if (listeners.TryGetValue(t, out events))
            {
                foreach (Action<IEvent> action in events)
                {
                    action(e);
                }
            }
        }
    }
}