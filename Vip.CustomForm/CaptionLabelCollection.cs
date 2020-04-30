using System.Collections;
using System.Windows.Forms;

namespace Vip.CustomForm
{
    public class CaptionLabelCollection : CollectionBase
    {
        private readonly Form Owner;

        public CaptionLabel this[int index] => (CaptionLabel) List[index];

        public CaptionLabelCollection(Form sender)
        {
            Owner = sender;
        }

        public bool Contains(CaptionLabel itemType)
        {
            return List.Contains(itemType);
        }

        public int Add(CaptionLabel itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            return List.Add(itemType);
        }

        public void Remove(CaptionLabel itemType)
        {
            List.Remove(itemType);
        }

        public void Insert(int index, CaptionLabel itemType)
        {
            itemType.SetOwner(Owner);
            if (string.IsNullOrEmpty(itemType.Name)) itemType.Name = GetUniqueName();
            List.Insert(index, itemType);
        }

        public int IndexOf(CaptionLabel itemType)
        {
            return List.IndexOf(itemType);
        }

        public CaptionLabel FindByName(string name)
        {
            foreach (CaptionLabel item in List)
                if (item.Name == name)
                    return item;
            return null;
        }

        protected override void OnInsert(int index, object value)
        {
            if (string.IsNullOrEmpty(((CaptionLabel) value).Name)) ((CaptionLabel) value).Name = GetUniqueName();
            base.OnInsert(index, value);
            ((CaptionLabel) value).SetOwner(Owner);
        }

        private string GetUniqueName()
        {
            int num = 1;
            while (Count != 0)
            {
                bool flag = true;
                int num2 = 0;
                while (num2 < Count)
                {
                    if (!(this[num2].Name == "CaptionLabel" + num))
                    {
                        num2++;
                        continue;
                    }

                    flag = false;
                    break;
                }

                if (flag) break;
                num++;
            }

            return "CaptionLabel" + num;
        }
    }
}