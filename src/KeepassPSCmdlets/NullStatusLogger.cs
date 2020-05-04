using KeePassLib.Interfaces;

namespace KeepassPSCmdlets
{
    internal class NullStatusLogger : IStatusLogger
    {
        public void StartLogging(string strOperation, bool bWriteOperationToLog)
        {
        }

        public void EndLogging()
        {
        }

        public bool SetProgress(uint uPercent)
        {
            return true;
        }

        public bool SetText(string strNewText, LogStatusType lsType)
        {
            return true;
        }

        public bool ContinueWork()
        {
            return true;
        }
    }
}