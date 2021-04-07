using System;
using System.Linq;
using System.Threading.Tasks;
using NDesk.Options;

class Program {
    
    public static async Task<int> Main (string[] args)
    {
        string cnpj = null, certificado = null, senha = null, cUf = null, chaveNfe = null, arquivo = null;
        bool ajuda = false;

        var p = new OptionSet () {
            { "cnpj=", "{CNPJ} da empresa sem máscara.", (long v) => cnpj = v.ToString() },
            { "uf=", "Código IBGE da {UF}.", (int v) => cUf = v.ToString() },
            { "c|certificado=", "Arquivo de {CERTIFICADO} digital.", v => certificado = v },
            { "s|senha=", "{SENHA} do certificado digital.", v => senha = v },
            { "n|nfe=", "{CHAVE} da NF-e.", v => chaveNfe = v },
            { "o|arquivo=", "{ARQUIVO} de saída.", v => arquivo = v },
            { "?|ajuda|h|help", "Exibe os parâmetros aceitos", v => ajuda = true }
        };

        var extra = p.Parse (args);
        
        if(extra.Any()){
            Console.Error.WriteLine($"O parâmetro '{extra.First()}' não foi reconhecido. Verifique se ele foi escrito corretamente.");
            ajuda = true;
        }

        if(ajuda || !args.Any()){
            Console.WriteLine ($"Uso: {System.AppDomain.CurrentDomain.FriendlyName} [Opções]");
            Console.WriteLine ();
            Console.WriteLine ("Opções:");
            p.WriteOptionDescriptions(Console.Out);
            return -2;
        }
        
        try
        {
            var sefaz = new Sefaz.Core.Sefaz(certificado, senha);

            var doc = await sefaz.BaixarNFe(cUf, cnpj, chaveNfe);
            if(string.IsNullOrWhiteSpace(arquivo)){
               Console.WriteLine(doc); 
            }else{
                doc.SalvarArquivo(arquivo);
            }
            return 0;
        }
        catch(ArgumentException e)
        {
            Console.Error.WriteLine(e.Message);
        }
        catch(Sefaz.Core.SefazException e)
        {
            Console.Error.WriteLine(e.Message);
        }
        
        return -1;
    }
}