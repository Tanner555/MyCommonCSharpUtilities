using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCommonUtilities
{
    class TimeUtility
    {
        float timerRate = 0.0f;
        double currentTimer = 0.0f;
        bool bPresentPastThreshold
        {
            get { return currentTimeInMilliseconds >= currentTimer; }
        }

        double currentTimeInMilliseconds
        {
            get
            {
                return
                  ((1 + DateTime.Now.Hour) * 60) +
                  ((1 + DateTime.Now.Minute) * 60) +
                  ((1 + DateTime.Now.Second) * 1000) +
                  DateTime.Now.Millisecond;
            }
        }

        public bool bTimerStartedCalled
        {
            get { return _bTimerStartedCalled; }
        }
        bool _bTimerStartedCalled = false;

        public TimeUtility(float rate)
        {
            timerRate = rate;
        }

        public void StartTimer()
        {
            _bTimerStartedCalled = true;
            currentTimer = currentTimeInMilliseconds + (timerRate * 1000);
        }

        public bool IsTimerFinished_Restart()
        {
            bool _finished = _bTimerStartedCalled && bPresentPastThreshold;
            if (_bTimerStartedCalled == false || _finished)
            {
                StartTimer();
            }
            return _finished;
        }
    }
}
