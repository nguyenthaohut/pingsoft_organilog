﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace TinyMVVM
{
    public class TinyViewModel : IDisposable, INotifyPropertyChanged
    {
        private NavigationPage _navigationPage;

        /// <summary>
        /// To be added
        /// </summary>
        public string CurrentNavigationServiceName = Constants.DefaultNavigationServiceName;

        /// <summary>
        /// To be added
        /// </summary>
        public string PreviousNavigationServiceName;

        /// <summary>
        /// The previous view model, that's automatically filled, on push
        /// </summary>
        public TinyViewModel PreviousViewModel { get; set; }

        /// <summary>
        /// This event is raise when a page is Popped, this might not be raise everytime a page is Popped.
        /// Note* this might be raised multiple times.
        /// </summary>
        public event EventHandler PageWasPopped;

        /// <summary>
        /// This property is used by the TinyContentPage and allows you to set the toolbar items on the page.
        /// </summary>
        public ObservableCollection<ToolbarItem> ToolbarItems { get; set; }

        /// <summary>
        /// A reference to the current page, that's automatically filled, on push
        /// </summary>
        public Page CurrentPage { get; set; }

        /// <summary>
        /// Core methods are basic built in methods for the App including Pushing, Pop and Alert
        /// </summary>
        public IViewModelCoreMethods CoreMethods { get; set; }

        /// <summary>
        /// This means the current ViewModel is shown modally and can be pop'd modally
        /// </summary>
        public bool IsModal { get; set; }

        /// <summary>
        /// Is true when this modal is the first of a new navigation stack
        /// </summary>
        public bool IsModalFirstChild { get; set; }

        /// <summary>
        /// This means the current ViewModel is shown modally and can be pop'd modally
        /// </summary>
        public bool IsModalAndHasPreviousNavigationStack => !string.IsNullOrWhiteSpace(PreviousNavigationServiceName) && PreviousNavigationServiceName != CurrentNavigationServiceName;

        private bool isBusy = false;

        /// <summary>
        /// This means the current page is busy
        /// </summary>
        public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }

        private string title = string.Empty;

        /// <summary>
        /// Page title
        /// </summary>
        public string Title { get => title; set => SetProperty(ref title, value); }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null, params object[] relatedProperties)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);

            foreach (var property in relatedProperties)
            {
                OnPropertyChanged(nameof(property));
            }

            return true;
        }

        public TinyViewModel()
        {
        }

        /// <summary>
        /// This method is called when the ViewModel is loaded, the initData is the data that's sent from ViewModel before
        /// </summary>
        /// <param name="data">Data that's sent to this PageModel from the pusher</param>
        public virtual void Init(NavigationParameters parameters)
        {
        }

        /// <summary>
        /// This method is called when a page is Pop'd, it also allows for data to be returned.
        /// </summary>
        /// <param name="returnedData">This data that's returned from </param>
        public virtual void ReverseInit(NavigationParameters parameters)
        {
        }

        /// <summary>
        /// This method is called when after page is created and bindingcontext is assigned to page.
        /// </summary>
        public virtual void OnPageCreated()
        {
        }

        /// <summary>
        /// This method is called when a page is Push'd or is set as page root in navigation stack.
        /// </summary>
        public virtual void OnPushed(NavigationParameters parameters)
        {
        }

        /// <summary>
        /// This method is called when a page is Pop'd.
        /// </summary>
        public virtual void OnPopped()
        {
        }

        public virtual void Dispose()
        {
            GC.Collect();
        }

        internal void WireEvents()
        {
            CurrentPage.Appearing += new WeakEventHandler<EventArgs>(ViewIsAppearing).Handler;
            CurrentPage.Disappearing += new WeakEventHandler<EventArgs>(ViewIsDisappearing).Handler;
        }

        protected virtual void ViewIsAppearing(object sender, EventArgs e)
        {
            if (!_alreadyAttached)
                AttachPageWasPoppedEvent();
        }

        private bool _alreadyAttached = false;

        /// <summary>
        /// This is used to attach the page was popped method to a NavigationPage if available
        /// </summary>
        private void AttachPageWasPoppedEvent()
        {
            if (CurrentPage.Parent is NavigationPage navPage)
            {
                _navigationPage = navPage;
                _alreadyAttached = true;
                navPage.Popped += new WeakEventHandler<NavigationEventArgs>(HandleNavPagePopped).Handler;
            }
        }

        protected bool IsDisposing;

        private void HandleNavPagePopped(object sender, NavigationEventArgs e)
        {
            if (e.Page == CurrentPage)
            {
                RaisePageWasPopped();
            }
        }

        public void RaisePageWasPopped()
        {
            IsDisposing = true;

            OnPopped();
            PageWasPopped?.Invoke(this, EventArgs.Empty);
            ReleaseResource();

            IsDisposing = false;
        }

        private void ReleaseResource()
        {
            if (CurrentPage.Parent is NavigationPage navPage)
                navPage.Popped -= HandleNavPagePopped;

            if (_navigationPage != null)
                _navigationPage.Popped -= HandleNavPagePopped;

            _navigationPage = null;

            CurrentPage.Appearing -= ViewIsAppearing;
            CurrentPage.Disappearing -= ViewIsDisappearing;
            CurrentPage.BindingContext = null;
        }

        protected virtual void ViewIsDisappearing(object sender, EventArgs e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}