namespace Maxi.BackOffice.CrossCutting.Common.Attributes
{
    public class PropEntityAtributes : System.Attribute
    {
        private bool insert;
        private bool update;
        private bool delete;
        private bool key;
        private string stringformat;
        private bool iscons;
        private string constante;
        private bool regDefault;
        private string label;
        private bool allownull;
        private Tipo tipoDato;

        public bool RegDefault
        {
            get { return regDefault; }
            set { regDefault = value; }
        }

        public string Stringformat
        {
            get { return stringformat; }
            set { stringformat = value; }
        }

        public PropEntityAtributes()
        {
            insert = true;
            update = true;
            delete = false;
            key = false;
            iscons = false;
            constante = null;
            RegDefault = false;
            AllowNull = false;
            TipoDato = Tipo.String;
        }


        public bool Insert
        {
            get { return insert; }
            set { insert = value; }
        }
        public bool Update
        {
            get { return update; }
            set { update = value; }
        }
        public bool Delete
        {
            get { return delete; }
            set { delete = value; }
        }

        public bool Key
        {
            get { return key; }
            set { key = value; }
        }

        public string Constante
        {
            get { return constante; }
            set { constante = value; }
        }



        public bool Iscons
        {
            get { return iscons; }
            set { iscons = value; }
        }

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        public bool AllowNull
        {
            get
            {
                return allownull;
            }

            set
            {
                allownull = value;
            }
        }

        public Tipo TipoDato
        {
            get => tipoDato;
            set => tipoDato = value;
        }
    }
}
