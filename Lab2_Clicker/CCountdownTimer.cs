namespace Lab3_ClickOrLoose
{
    public class CCountdownTimer
    {
        private double targetTime;
        private double currentTime;

        public CCountdownTimer(double timeBeforeClick)
        {
            targetTime = timeBeforeClick;
            currentTime = 0;
        }

        public void Update(double delta)
        {
            if (currentTime < targetTime)
                currentTime += delta;
        }

        public bool IsReady()
        {
            return currentTime >= targetTime;
        }

        public void Reset()
        {
            currentTime = 0;
        }

        public void DecreaseTargetTime(double amount)
        {
            targetTime -= amount;
            if (targetTime < 0.1) targetTime = 0.1; // защита от 0 или минуса
        }
        public double GetTargetTime()
        {
            return targetTime;
        }
    }
}