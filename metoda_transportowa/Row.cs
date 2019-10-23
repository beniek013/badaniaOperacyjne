using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metoda_transportowa
{
    public class Row
    {
        public int id;
        public string dostawca;
        public int popyt;
        public string odbiorca;
        public int podaż;
        public int odleglosc;
        public bool enabled;
        public int? result; 

        public Row(int iD, string d, int pop, string o, int pod, bool ena)
        {
            id = iD;
            dostawca = d;
            popyt = pop;
            odbiorca = o;
            podaż = pod;
            enabled = ena;
            
        }
    }
}
