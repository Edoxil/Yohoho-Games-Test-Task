namespace Game
{
    [System.Serializable]
    public struct TimerComponenet
    {
        private float _elapsedTime;
        public float time;

        public bool Elapsed => _elapsedTime >= time;

        public void AddTime(float additionalTime)
        {
            _elapsedTime += additionalTime;
        }

        public void Reset()
        {
            _elapsedTime = 0f;
        }
    }

}