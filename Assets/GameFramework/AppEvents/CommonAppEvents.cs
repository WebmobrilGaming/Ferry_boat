using System;
using System.Collections;
using System.Reflection;
using Netconfig;
using UnityEngine;

namespace GF
{
    public class YesNoPopupEvent : GameEvent
    {
        public Action yesAction;
        public Action noAction;
        public string title;
        public string info;
        public YesNoPopupEvent(string title, string info, Action yes, Action no)
        {
            this.yesAction = yes;
            this.noAction = no;
            this.title = title;
            this.info = info;
        }
    }

    public class OkPopupEvent : GameEvent
    {
        public Action okAction;
        public string title;
        public string info;
        public OkPopupEvent(string title, string info, Action ok)
        {
            this.title = title;
            this.info = info;
            this.okAction = ok;
        }
    }
    public class CoroutineEvent : GameEvent
    {
        private IEnumerator _enumerator;
        public IEnumerator Enumerator { get { return _enumerator; } }
        public CoroutineEvent(IEnumerator enumerator)
        {
            _enumerator = enumerator;
        }
    }
    public class ApiEvent : GameEvent
    {
        public  Request request;
        public HttpRequestType httpRequestType;
        public string url;
        public Action<Response> responseCallback;
        public ApiEvent(HttpRequestType httpRequestType,Request request, Action<Response> responseCallback)
        {
            this.request = request;
            this.httpRequestType=httpRequestType;
            this.responseCallback = responseCallback;
        }
    }

    public class RegisterCustomServiceEvent : GameEvent
    {
        public string NameSpaceType;
        private Assembly assembly;
        public Assembly Assembly => string.IsNullOrEmpty(assemblyName) ? Assembly.GetExecutingAssembly() : Assembly.Load(assemblyName);
        public string assemblyName;
        public RegisterCustomServiceEvent(string nameSpaceType, string assembly)
        {
            this.NameSpaceType = nameSpaceType;
            this.assemblyName = assembly;
        }
    }
}