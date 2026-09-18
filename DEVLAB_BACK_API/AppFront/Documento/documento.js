const URL_API = 'https://localhost:7081/api/v1/Documento';
async function buscarDocumentos() {

    const codigoCliente = document.getElementById("codigoCliente").value;

    if (!codigoCliente) {
        alert("Informe o código do cliente.");
        return;
    }

    try {

        const response = await fetch(
            `${URL_API}/cliente/${codigoCliente}`
        );

        if (!response.ok) {
            throw new Error("Erro ao buscar documentos.");
        }

        const documentos = await response.json();

        preencherTabela(documentos);

    } catch (erro) {

        console.error(erro);

        alert("Não foi possível buscar os documentos.");
    }
}


function preencherTabela(documentos) {

    const corpoTabela = document.getElementById("corpoTabela");

    corpoTabela.innerHTML = "";

    if (!documentos || documentos.length === 0) {

        corpoTabela.innerHTML = `
            <tr>
                <td colspan="4">
                    Nenhum documento encontrado.
                </td>
            </tr>
        `;

        return;
    }

    documentos.forEach(documento => {

        const linha = document.createElement("tr");

        linha.innerHTML = `
            <td>${documento.id}</td>
            <td>${documento.nome}</td>
            <td>${documento.extensao}</td>

            <td>

                <button 
                    class="btn-baixar"
                    onclick="baixarDocumento('${documento.id}')">
                    Baixar
                </button>

                <button 
                    class="btn-excluir"
                    onclick="excluirDocumento('${documento.id}')">
                    Excluir
                </button>

            </td>
        `;

        corpoTabela.appendChild(linha);

    });
}


async function enviarDocumento() {

    const codigoCliente =
        document.getElementById("codigoCliente").value;

    const inputArquivo =
        document.getElementById("arquivo");

    const arquivo = inputArquivo.files[0];

    if (!codigoCliente || !arquivo) {

        alert("Informe o código do cliente e selecione um arquivo.");

        return;
    }

    const dadosArquivo = new FormData();

    dadosArquivo.append("arquivo", arquivo);

    try {

        const response = await fetch(
            `${URL_API}/upload/${codigoCliente}`,
            {
                method: "POST",
                body: dadosArquivo
            }
        );

        if (response.ok) {

            alert("Documento enviado com sucesso!");

            document.getElementById("arquivo").value = "";

            await buscarDocumentos();

        } else {

            const erro = await response.json();

            alert(
                "Erro: " +
                (erro.message || "Falha ao enviar o documento.")
            );
        }

    } catch (erro) {

        console.error(erro);

        alert("Erro ao conectar com a API.");
    }
}

async function baixarDocumento(id) {

    try {

        const response = await fetch(
            `${URL_API}/download/${id}`
        );

        if (!response.ok) {

            throw new Error("Erro ao baixar o documento.");
        }

        const blob = await response.blob();

        const url = window.URL.createObjectURL(blob);

        const link = document.createElement("a");

        link.href = url;

        link.download = `documento-${id}`;

        document.body.appendChild(link);

        link.click();

        link.remove();

        window.URL.revokeObjectURL(url);

    } catch (erro) {

        console.error(erro);

        alert("Não foi possível baixar o documento.");
    }
}
async function excluirDocumento(id) {

    const confirmar = confirm(
        "Deseja realmente excluir este documento?"
    );

    if (!confirmar) {
        return;
    }

    try {

        const response = await fetch(
            `${URL_API}/${id}`,
            {
                method: "DELETE"
            }
        );

        if (!response.ok) {

            throw new Error("Erro ao excluir documento.");
        }

        alert("Documento excluído com sucesso!");

        await buscarDocumentos();

    } catch (erro) {

        console.error(erro);

        alert("Não foi possível excluir o documento.");
    }
}