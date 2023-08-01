using System.Collections.Generic;

public class Intent
{
    public double confidence { get; set; }
    public string id { get; set; }
    public string name { get; set; }
}

public class IntentRoot
{
    public List<Intent> intents { get; set; }
    public string text { get; set; }
}