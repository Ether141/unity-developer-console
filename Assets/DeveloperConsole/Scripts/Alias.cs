namespace Developer
{
    [System.Serializable]
    public struct Alias
    {
        public string alias;
        public string associatedCommand;

        public Alias(string alias, string associatedCommand)
        {
            this.alias = alias;
            this.associatedCommand = associatedCommand;
        }
    }
}
