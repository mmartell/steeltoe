﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Steeltoe.CircuitBreaker.Hystrix.Util;
using Steeltoe.Common.Util;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Steeltoe.CircuitBreaker.Hystrix.Metric.Consumer
{
    public abstract class BucketedCumulativeCounterStream<Event, Bucket, Output> : BucketedCounterStream<Event, Bucket, Output>
        where Event : IHystrixEvent
    {
        private readonly AtomicBoolean isSourceCurrentlySubscribed = new AtomicBoolean(false);
        private readonly BehaviorSubject<Output> counterSubject;
        private IObservable<Output> sourceStream;

        protected BucketedCumulativeCounterStream(IHystrixEventStream<Event> stream, int numBuckets, int bucketSizeInMs, Func<Bucket, Event, Bucket> reduceCommandCompletion, Func<Output, Bucket, Output> reduceBucket)
            : base(stream, numBuckets, bucketSizeInMs, reduceCommandCompletion)
        {
            this.counterSubject = new BehaviorSubject<Output>(EmptyOutputValue);
            this.sourceStream = bucketedStream
                    .Scan(EmptyOutputValue, (arg1, arg2) => reduceBucket(arg1, arg2))
                    .Skip(numBuckets)
                    .OnSubscribe(() => { isSourceCurrentlySubscribed.Value = true; })
                    .OnDispose(() => { isSourceCurrentlySubscribed.Value = false; })
                    .Publish().RefCount();           // multiple subscribers should get same data
        }

        public override IObservable<Output> Observe()
        {
            return sourceStream;
        }

        public void StartCachingStreamValuesIfUnstarted()
        {
            if (subscription.Value == null)
            {
                // the stream is not yet started
                IDisposable candidateSubscription = Observe().Subscribe(this.counterSubject);
                if (subscription.CompareAndSet(null, candidateSubscription))
                {
                    // won the race to set the subscription
                }
                else
                {
                    // lost the race to set the subscription, so we need to cancel this one
                    candidateSubscription.Dispose();
                }
            }
        }

         // Synchronous call to retrieve the last calculated bucket without waiting for any emissions
         // return last calculated bucket
        public Output Latest
        {
            get
            {
                if (counterSubject.TryGetValue(out Output v))
                {
                    return v;
                }
                else
                {
                    return EmptyOutputValue;
                }
            }
        }
    }
}
