﻿namespace Xamarin.Forms.Behaviors
{
    public class AddEffectBehavior : Behavior<View>
    {
        public static readonly BindableProperty GroupProperty = BindableProperty.Create(nameof(Group), typeof(string), typeof(AddEffectBehavior), null);

        public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(AddEffectBehavior), null);

        public string Group
        {
            get => (string)GetValue(GroupProperty);
            set => SetValue(GroupProperty, value);
        }

        public string Name
        {
            get => (string)GetValue(NameProperty);
            set => SetValue(NameProperty, value);
        }

        protected override void OnAttachedTo(BindableObject bindable)
        {
            base.OnAttachedTo(bindable);
            EffectAdd(bindable as View);
        }

        protected override void OnDetachingFrom(BindableObject bindable)
        {
            EffectRemove(bindable as View);
            base.OnDetachingFrom(bindable);
        }

        private void EffectAdd(View view)
        {
            var effect = GetEffect();
            if (effect == null || view == null)
                return;

            view.Effects.Add(effect);
        }

        private void EffectRemove(View view)
        {
            var effect = GetEffect();
            if (effect == null || view == null)
                return;

            view.Effects.Remove(effect);
        }

        private Effect GetEffect()
        {
            if (!string.IsNullOrWhiteSpace(Group) && !string.IsNullOrWhiteSpace(Name))
            {
                return Effect.Resolve(string.Format("{0}.{1}", Group, Name));
            }
            return null;
        }
    }
}