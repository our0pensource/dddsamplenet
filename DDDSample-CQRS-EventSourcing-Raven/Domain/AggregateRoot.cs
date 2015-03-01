using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace DDDSample.Domain
{
   [Serializable]
   public abstract class AggregateRoot : IAggregateRoot
   {      
      protected void Publish<TAggregate, TEvent>(TAggregate @this, TEvent @event)
         where TAggregate : AggregateRoot
         where TEvent : Event<TAggregate>
      {
         Apply(@event);
         ((IAggregateRoot)this).Events.Add(@event);
         _currentVersion++;
         Bus.Publish(@this, @event);
      }

      private void Apply(object @event)
      {
         if (@event == null)
         {
            throw new ArgumentNullException("event");
         }
         string eventTypeName = @event.GetType().Name;
         int suffixIndex = eventTypeName.LastIndexOf("Event");
         if (suffixIndex <= 0)
         {
            throw new InvalidOperationException("Invalid event name: " + eventTypeName);
         }
         string methodName = "On" + eventTypeName.Substring(0, suffixIndex);
         MethodInfo methodInfo = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
         methodInfo.Invoke(this, new[] { @event });
      }

      void IAggregateRoot.LoadFromEventStream(IEnumerable<object> events)
      {
         foreach (object @event in events)
         {
            Apply(@event);
         }
      }

      [JsonIgnore]
      public string Id
      {
         get { return _id; }
         set { _id = value; }
      }

      [JsonIgnore]
      int IAggregateRoot.OriginalVersion
      {
         get { return _originalVersion; }
         set { _originalVersion = value; }
      }

      [JsonIgnore]
      int IAggregateRoot.CurrentVersion
      {
         get { return _currentVersion; }
         set { _currentVersion = value; }
      }

      [JsonIgnore]
      ICollection<object> IAggregateRoot.Events
      {
         get
         {
            if (_events == null)
            {
               _events = new List<object>();
            }
            return _events;
         }
      }

      [NonSerialized] 
      [JsonIgnore]
      private List<object> _events;
      [NonSerialized]
      [JsonIgnore]
      private string _id = Guid.NewGuid().ToString();
      [NonSerialized]
      [JsonIgnore]
      private int _originalVersion;
      [NonSerialized]
      [JsonIgnore]
      private int _currentVersion;
   }
}