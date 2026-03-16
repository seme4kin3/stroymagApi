
using System.Text.RegularExpressions;
using System.Text;

namespace Domain.Catalog
{
    public class AttributeDefinition
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; private set; }      // "Мощность"
        public string Key { get; private set; }       // "power"
        public AttributeDataType DataType { get; private set; }

        /// <summary>Активен ли атрибут (мягкое удаление).</summary>
        public bool IsActive { get; private set; } = true;
        public virtual ICollection<CategoryAttribute> CategoryAttributes { get; private set; } = new List<CategoryAttribute>();
        public virtual ICollection<ProductAttributeValue> ProductValues { get; private set; } = new List<ProductAttributeValue>();

        private AttributeDefinition() { }

        public AttributeDefinition(string name, AttributeDataType dataType)
        {
            SetName(name);
            SetKey(GenerateKeyFrom(name));
            DataType = dataType;
        }

        public void Rename(string name) => SetName(name);

        public void ChangeKey(string key) => SetKey(key);

        public void ChangeType(AttributeDataType dataType) => DataType = dataType;


        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Attribute name is required", nameof(name));
            Name = name.Trim();
        }

        private void SetKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Attribute key is required", nameof(key));
            Key = key.Trim().ToLowerInvariant();
        }

        private static string GenerateKeyFrom(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "category";

            var slug = Transliterate(name.Trim().ToLowerInvariant());

            var sb = new StringBuilder();
            foreach (var ch in slug)
            {
                if ((ch >= 'a' && ch <= 'z') ||
                    (ch >= '0' && ch <= '9'))
                {
                    sb.Append(ch);
                }
                else
                {
                    sb.Append('-');
                }
            }

            var res = Regex.Replace(sb.ToString(), "-{2,}", "-")
                           .Trim('-');

            return string.IsNullOrWhiteSpace(res) ? "category" : res;
        }

        private static string Transliterate(string input)
        {
            var map = new Dictionary<char, string>
            {
                ['а'] = "a",
                ['б'] = "b",
                ['в'] = "v",
                ['г'] = "g",
                ['д'] = "d",
                ['е'] = "e",
                ['ё'] = "e",
                ['ж'] = "zh",
                ['з'] = "z",
                ['и'] = "i",
                ['й'] = "y",
                ['к'] = "k",
                ['л'] = "l",
                ['м'] = "m",
                ['н'] = "n",
                ['о'] = "o",
                ['п'] = "p",
                ['р'] = "r",
                ['с'] = "s",
                ['т'] = "t",
                ['у'] = "u",
                ['ф'] = "f",
                ['х'] = "h",
                ['ц'] = "ts",
                ['ч'] = "ch",
                ['ш'] = "sh",
                ['щ'] = "sch",
                ['ъ'] = "",
                ['ы'] = "y",
                ['ь'] = "",
                ['э'] = "e",
                ['ю'] = "yu",
                ['я'] = "ya"
            };

            var sb = new StringBuilder();

            foreach (var ch in input)
            {
                if (map.TryGetValue(ch, out var val))
                    sb.Append(val);
                else
                    sb.Append(ch);
            }

            return sb.ToString();
        }
    }
}
