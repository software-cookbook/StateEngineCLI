namespace StateEngineCLI
{
    public class StateEngineConfig
    {
        public Attribute[] attributes { get; set; }
        public State[] states { get; set; }
    }

    public class Attribute
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public class State
    {
        public string name { get; set; }
        public string type { get; set; }
        public bool? start { get; set; } = false;
        public Transition[] transitions { get; set; }
    }

    public class Transition
    {
        public string action { get; set; }
        public string nextState { get; set; }
    }


}
