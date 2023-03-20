namespace LMFS
{
    class ConsoleCache
    {
        public string str = "";
        public int Index = 0;
        public int LineW = 0;
        public void AddChar(char c)
        {
            if (Index == str.Length)
            {
                str += c;
            }
            else
                str = str.Insert(Index, "" + c);
        }
        public void Remove()
        {
            var l = str.ToList();
            if (Index - 1 >= 0 && Index - 1 < str.Length)
            {
                l.RemoveAt(Index - 1);
                str = new string(l.ToArray());
                Index--;
            }
        }
    }
}