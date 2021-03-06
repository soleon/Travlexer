﻿using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Codify.TriggerActions
{
    /// <summary>
    ///     Focus on the target object.
    /// </summary>
    public class FocusAction : TargetedTriggerAction<Control>
    {
        protected override void Invoke(object parameter)
        {
            if (Target == null)
            {
                return;
            }
            Target.Focus();
        }
    }
}