using PM.Shared.Dtos.cores;
using PM.Shared.Handle.Interfaces;

namespace PM.Shared.Handle.Implements
{
    public class StatusHandle
    {
        private DateTime _startAt;
        private DateTime _endAt;
        private bool _isCompleted;
        public StatusHandle(DateTime startAt, DateTime endAt, bool isCompleted)
        {
            _endAt = endAt;
            _startAt = startAt;
            _isCompleted = isCompleted;
        }
        public int GetStatus()
        {
            if(_startAt == _endAt) return (int) TypeStatus.NotSelected;
            if (DateTime.Now < _startAt) return (int)TypeStatus.Waiting;
            if (DateTime.Now >= _startAt && DateTime.Now < _endAt) return (int)TypeStatus.InProgress;
            if (!_isCompleted)
            {
                
                if (_endAt > DateTime.Now) return (int)TypeStatus.BehindSchedule;
            }
            else
            {
                if(_endAt < DateTime.Now) return (int) TypeStatus.CompletedEarly;
                if(_endAt > DateTime.Now) return (int) TypeStatus.FinishedLate;
                if (_endAt == DateTime.Now) return (int)TypeStatus.FinishedOnTime;
            }
            return (int)TypeStatus.Node;
        }
    }
}
