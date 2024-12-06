namespace TestJordy.Models;

public class Classes
{
    
}public class RootObject
{
    public string _context { get; set; }
    public Adressen[] adressen { get; set; }
    public string volgende { get; set; }
}

public class Adressen
{
    public string _type { get; set; }
    public Identificator identificator { get; set; }
    public string detail { get; set; }
    public string huisnummer { get; set; }
    public VolledigAdres volledigAdres { get; set; }
    public string adresStatus { get; set; }
}

public class Identificator
{
    public string id { get; set; }
    public string naamruimte { get; set; }
    public string objectId { get; set; }
    public string versieId { get; set; }
}

public class VolledigAdres
{
    public GeografischeNaam geografischeNaam { get; set; }
}

public class GeografischeNaam
{
    public string spelling { get; set; }
    public string taal { get; set; }
}

