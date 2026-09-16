namespace BancoSENAIAPI.Models
{
    public class DocumentoMetadado
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public string Extensao { get; set;}
        public string Caminho { get; set;}
        public int CodigoCliente { get; set;}

        internal static object Where(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }
    }
}
