using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace GF.TutorialSystem
{
    public interface ISequence
    {
        void Execute(Action complete);
    }
    [Serializable]
    public class TutorialSequence : ISequence
    {
        private Action onCompleteCallback;
        public virtual void Execute(Action onCompleteCallback)
        {
            this.onCompleteCallback = onCompleteCallback;
        }
        protected void Complete()
        {
            this.onCompleteCallback?.Invoke();
            Debug.Log($"Sequence Completed");
        }
    }
    [Serializable]
    public class TutorialController : ISequence
    {
        private Action completeCallback;
        private TutorialSequence[] tutorialSequences;
        private Queue<ISequence> sequenceQueue;
        public void Initialize(TutorialSequence[] tutorialSequences)
        {
            this.tutorialSequences = tutorialSequences;
        }
        public virtual void Execute(Action completeCallback)
        {
            this.completeCallback = completeCallback;
            sequenceQueue = new Queue<ISequence>();
            foreach (var sequence in tutorialSequences)
            {
                sequenceQueue.Enqueue(sequence);
            }
            ExecuteQueue();
        }
        private void ExecuteQueue()
        {
            if (sequenceQueue.Count <= 0)
            {
                this.completeCallback?.Invoke();
                return;
            }
            var sequence = sequenceQueue.Dequeue();
            sequence.Execute(ExecuteQueue);
        }
    }
}