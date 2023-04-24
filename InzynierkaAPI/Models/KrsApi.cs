namespace InzynierkaAPI.Models
{
#pragma warning disable IDE1006 // Style nazewnictwa
    public class KrsApi
    {
        public virtual odpis odpis { get; set; }
        
    }
    public class odpis
    {
        public virtual naglowekA naglowekA { get; set; }
        public virtual dane dane { get; set; }


    }

    public class naglowekA
    {
        public string numerKRS { get; set; }

    }
    public class dane
    {
        public virtual dzial1 dzial1 { get; set; }
    }
    public class dzial1
    {
        public virtual danePodmiotu danePodmiotu { get; set; }
        public virtual siedzibaIAdres siedzibaIAdres { get; set; }
    }
    public class danePodmiotu
    {
        public string nazwa { get; set; }
        public virtual identyfikatory identyfikatory { get; set; }
    }

    public class identyfikatory
    {
        public string nip { get; set; }
        public string regon { get; set; }
    }

    public class siedzibaIAdres
    {
        public virtual adres adres { get; set; }
    }
    public class adres
    {
        public string ulica { get; set; }
        public string nrDomu { get; set; }
        public string miejscowosc { get; set; }
        public string kodPocztowy { get; set; }

    }
#pragma warning restore IDE1006 // Style nazewnictwa

}
