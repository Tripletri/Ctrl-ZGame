using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace CtrlZ
{
    internal class InputManager
    {
        public readonly List<Bind> Binds = new List<Bind>();
        private readonly HashSet<Key> heldKeys = new HashSet<Key>();

        public void AddBind(Key key, Action action, bool allowHold = true, string description = null)
        {
            Binds.Add(new Bind(key, action, allowHold, description));
        }

        public void ProceedControl()
        {
            foreach (var bind in Binds)
                if (Keyboard.IsKeyDown(bind.Key))
                {
                    if (bind.AllowHold || !heldKeys.Contains(bind.Key))
                    {
                        bind.Action();
                        if (!bind.AllowHold)
                            heldKeys.Add(bind.Key);
                    }
                }
                else if (heldKeys.Contains(bind.Key))
                    heldKeys.Remove(bind.Key);
        }
    }

    internal class Bind
    {
        public Bind(Key key, Action action, bool allowHold, string description = null)
        {
            Key = key;
            Action = action;
            AllowHold = allowHold;
            Description = description;
        }

        public Key Key { get; set; }
        public Action Action { get; set; }
        public bool AllowHold { get; set; }

        public string Description { get; private set; }
    }
}