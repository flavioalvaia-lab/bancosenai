
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly string _caminhoRaiz =
            Path.Combine(Directory.GetCurrentDirectory(), "ClienteArquivos");

        private static List<DocumentoMetadado> _documentosMetadados =
            new List<DocumentoMetadado>();

        private static int _nextId = 1;

        private const long TAMANHO_MAXIMO = 2 * 1024 * 1024;

        private readonly string[] extensoesPermitidas =
        {
            ".pdf",
            ".jpg",
            ".png"
        };


        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(
            int codigoCliente,
            IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado.");
            }

            if (arquivo.Length > TAMANHO_MAXIMO)
            {
                return BadRequest(
                    "O arquivo excede o limite máximo permitido de 2 MB.");
            }

            string extensao =
                Path.GetExtension(arquivo.FileName).ToLower();


            if (!extensoesPermitidas.Contains(extensao))
            {
                return BadRequest(
                    "Extensão de arquivo não permitida. " +
                    "Apenas arquivos .pdf, .jpg e .png são aceitos.");
            }

            string pastaCliente =
                Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string nomeOriginal =
                Path.GetFileNameWithoutExtension(arquivo.FileName);

            string novoNome =
                $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";

            string caminhoFinal =
                Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(
                caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadados = new DocumentoMetadado
            {
                Id = _nextId++,
                Name = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };

            _documentosMetadados.Add(documentoMetadados);

            return Ok(new
            {
                mensagem = "Documento anexado com sucesso!",
                arquivoSalvo = novoNome
            });
        }


        [HttpGet("listar/{codigoCliente}")]
        public IActionResult ListarDocumentos(int codigoCliente)
        {
            var documentos = _documentosMetadados
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToList();

            if (!documentos.Any())
            {
                return NotFound(new
                {
                    message = "Nenhum documento encontrado para este cliente."
                });
            }

            return Ok(documentos);
        }


        [HttpGet("download/{id}")]
        public IActionResult Download(int id)
        {
            var documento = _documentosMetadados
                .FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound(new
                {
                    message = "Documento não encontrado."
                });
            }

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound(new
                {
                    message = "Arquivo não encontrado no servidor."
                });
            }

            byte[] fileBytes =
                System.IO.File.ReadAllBytes(documento.Caminho);

            string nomeArquivo =
                documento.Name + documento.Extensao;

            return File(
                fileBytes,
                "application/octet-stream",
                nomeArquivo);
        }


        [HttpDelete("excluir/{id}")]
        public IActionResult Excluir(int id)
        {
            var documento = _documentosMetadados
                .FirstOrDefault(d => d.Id == id);

            if (documento == null)
            {
                return NotFound(new
                {
                    message = "Documento não encontrado."
                });
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _documentosMetadados.Remove(documento);

            return Ok(new
            {
                message = "Documento excluído com sucesso."
            });
        }
    }
}