# 🛍️ API de Otimização de Empacotamento - Loja do Seu Manoel

API desenvolvida para a Avaliação Técnica .NET, com o objetivo de automatizar o processo de seleção de caixas para embalagem de pedidos, visando a otimização do espaço e a minimização do número de caixas utilizadas.

## 📝 Descrição do Projeto

A "Loja do Seu Manoel" é uma loja de jogos online que necessita automatizar o processo de embalagem dos seus pedidos. Esta API WEB recebe, em formato JSON, uma lista de pedidos. Cada pedido contém uma lista de produtos, cada um com suas dimensões (altura, largura, comprimento). A API processa cada pedido, determina a melhor forma de empacotar os produtos utilizando um conjunto de caixas pré-fabricadas disponíveis, e retorna quais caixas devem ser usadas para cada pedido e quais produtos irão em cada caixa.

## 🛠️ Tecnologias Utilizadas

* **Backend:** C# com .NET 8 (ASP.NET Core Web API)
* **Banco de Dados:** SQL Server (executando em contentor Docker)
* **ORM:** Entity Framework Core 8 (ou 9 Preview, conforme versão dos pacotes)
* **Contentorização:** Docker e Docker Compose
* **Documentação da API:** Swagger (OpenAPI)
* **Testes Unitários:** MSTest e Moq
* **IDE:** Visual Studio 2022

[![CSharp](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/pt-br/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://docs.microsoft.com/pt-br/aspnet/core/)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?style=for-the-badge&logo=.net&logoColor=white)](https://docs.microsoft.com/pt-br/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/pt-br/sql-server)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)
[![MSTest](https://img.shields.io/badge/MSTest-0078D4?style=for-the-badge&logo=visualstudio&logoColor=white)](https://docs.microsoft.com/pt-br/visualstudio/test/getting-started-with-unit-testing)
[![Moq](https://img.shields.io/badge/Moq-992292?style=for-the-badge)](https://github.com/moq)

## 🚀 Pré-requisitos para Execução

* Docker Desktop instalado e em execução na sua máquina.
    * Disponível em: [https://www.docker.com/products/docker-desktop/](https://www.docker.com/products/docker-desktop/)

## ⚙️ Como Configurar e Executar a Aplicação

1.  **Clone o Repositório:**
    Abra o seu terminal e execute os seguintes comandos. O primeiro comando clona o repositório do GitHub para a sua máquina local, criando uma pasta chamada `ApiDeOtimizacaoDeEmpacotamentoDoSeuManoel`. O segundo comando entra nessa pasta recém-criada, que é a raiz do projeto.

    ```bash
    git clone https://github.com/MatheusDateu/ApiDeOtimizacaoDeEmpacotamentoDoSeuManoel.git
    cd ApiDeOtimizacaoDeEmpacotamentoDoSeuManoel

    ```
    Após estes comandos, você estará na pasta raiz do projeto, onde deverá encontrar o ficheiro `docker-compose.yml` e o ficheiro da solução (ex: `ManoelsPackagingOptimiser.sln`). Todos os comandos subsequentes (como `docker-compose up`) devem ser executados a partir desta pasta.

2.  **Construa e Inicie os Contentores Docker:**
    Na pasta raiz da solução (onde o ficheiro `docker-compose.yml` está localizado), execute o seguinte comando no seu terminal:
    ```shell
    docker-compose up --build -d
    ```
    Este comando irá:
    * Construir a imagem Docker para a API (se for a primeira vez ou se houver alterações no código).
    * Descarregar a imagem Docker do SQL Server.
    * Iniciar ambos os contentores (API e Base de Dados) em segundo plano (`-d`).

3.  **Migrações da Base de Dados:**
    A base de dados `ManoelsPackagingDb` e o seu esquema (tabelas `Boxes` e `DimensionsSet`, incluindo os dados iniciais das caixas) são criados e as migrações do Entity Framework Core são aplicadas **automaticamente** quando a API inicia pela primeira vez (devido à configuração no `Program.cs`). Aguarde alguns segundos após os contentores subirem para que este processo seja concluído. Você pode verificar os logs do contentor da API para confirmar se a migração foi bem-sucedida.

## 🧪 Como Testar a API

1.  **Aceda ao Swagger UI:**
    Após os contentores estarem em execução e a base de dados migrada, abra o seu navegador de internet e aceda à interface do Swagger em:
    [http://localhost:8080/swagger](http://localhost:8080/swagger)
    *(A API está configurada para escutar na porta 8080, que é mapeada para a porta 8080 do seu computador pelo `docker-compose.yml`)*.

2.  **Execute um Pedido de Empacotamento:**
    * Na interface do Swagger, localize o controlador `Packaging` e o endpoint `POST /api/packaging/calculate`.
    * Clique em "Try it out".
    * No campo "Request body", insira um JSON com a lista de pedidos e produtos.
        **Exemplo de JSON de Entrada:**
        ```json
        {
          "pedidos": [
            {
              "pedido_id": 3,
              "produtos": [
                {
                  "produto_id": "Headset",
                  "dimensoes": { "altura": 25, "largura": 15, "comprimento": 20 }
                }
              ]
            },
            {
              "pedido_id": 4,
              "produtos": [
                {
                  "produto_id": "Mouse Gamer",
                  "dimensoes": { "altura": 5, "largura": 8, "comprimento": 12 }
                },
                {
                  "produto_id": "Teclado Mecânico",
                  "dimensoes": { "altura": 4, "largura": 45, "comprimento": 15 }
                }
              ]
            }
          ]
        }
        ```
    * Clique no botão "Execute".

3.  **Analise a Resposta:**
    A resposta será um JSON no formato `PackagingBatchResponse`, detalhando as caixas alocadas para cada pedido e os produtos contidos nelas.

## 🧠 Lógica de Empacotamento Implementada (Heurística)

A API utiliza a seguinte heurística para determinar o empacotamento dos produtos de cada pedido no serviço `PackagingOptimisationService`:
1.  Os produtos de cada pedido são convertidos para entidades de domínio e inicialmente ordenados por volume, do maior para o menor. Esta lista de produtos a embalar é processada iterativamente.
2.  Em cada iteração principal, o maior produto restante que ainda precisa ser embalado é selecionado.
3.  O algoritmo então avalia cada tipo de caixa disponível (obtida da base de dados e ordenada da menor para a maior por volume) como uma candidata para este produto inicial e outros que possam caber com ele:
    a.  Verifica-se se o produto inicial cabe na caixa candidata (considerando 6 orientações de rotação do produto).
    b.  Se couber, uma simulação de preenchimento é realizada para esta caixa candidata: o produto inicial é adicionado, e o seu volume é contabilizado.
    c.  Em seguida, os restantes produtos do pedido (ordenados do menor para o maior por volume) são testados um a um para ver se também cabem nesta mesma caixa candidata. Um produto adicional é adicionado se:
        i.  Ele individualmente cabe nas dimensões originais da caixa candidata (considerando rotações).
        ii. O seu volume, somado ao volume já ocupado na caixa candidata, não excede a capacidade total de volume da caixa.
    d.  Após simular o preenchimento para uma caixa candidata, o algoritmo compara este resultado (priorizando o maior número de itens embalados e, secundariamente, o maior volume total embalado) com o melhor resultado encontrado até agora nesta rodada (para outras caixas candidatas).
4.  A caixa candidata que resultou no "melhor" empacotamento (conforme o critério acima) para a rodada atual é escolhida. Os produtos alocados a ela são removidos da lista de produtos a embalar.
5.  O processo repete-se com os produtos restantes até que todos sejam embalados ou marcados como não embaláveis.
6.  Se um produto não couber em nenhuma caixa disponível (mesmo sozinho), ele é listado separadamente com uma observação.

**Limitações Conhecidas da Heurística Atual:**
* A verificação de encaixe de múltiplos produtos numa mesma caixa, durante a fase de "preenchimento guloso", é baseada na capacidade de cada produto subsequente caber individualmente no *tipo* de caixa original e na limitação do volume total. Não é realizada uma análise geométrica 3D complexa para garantir que o *conjunto* de todos os produtos pode ser fisicamente arranjado no espaço restante da caixa de forma otimizada. Esta é uma simplificação comum para um problema de "bin packing 3D", que é computacionalmente complexo (NP-difícil).
* A escolha da "melhor" caixa e a ordem de preenchimento são heurísticas e podem não produzir a solução matematicamente ótima em todos os cenários possíveis em termos de minimização absoluta do número de caixas ou do espaço desperdiçado.
* A estratégia de seleção da caixa inicial (menor que caiba o primeiro item, e depois a simulação) e o preenchimento guloso podem levar a ótimos locais que não são o ótimo global.

## Como Executar os Testes Unitários

1.  Navegue para a pasta do projeto de testes unitários (ex: `ManoelsPackaging.Api\ManoelsPackaging.Core.UnitTests` com esse comando:
    ```
    cd MonoelsPackaging.Core.UnitTests
    ```
2.  Para executar os testes unitários execute o seguinte comando:
    ```shell
    dotnet test
    ```

## 🏛️ Estrutura do Projeto (Simplificada)
```
ManoelsPackagingOptimiser/
├── src/
│   └── ManoelsPackaging.Api/         # Projeto Principal da API ASP.NET Core
│       ├── Contracts/                # DTOs para Requests e Responses
│       ├── Controllers/              # Controladores da API
│       ├── Core/                     # Lógica de Negócio e Entidades de Domínio
│       │   ├── Entities/
│       │   ├── Interfaces/           # Interfaces para Repositórios e Serviços
│       │   └── Services/             # Implementações dos Serviços de Negócio
│       ├── Infrastructure/           # Implementações de acesso a dados, etc.
│       │   ├── Persistence/
│       │   │   ├── Migrations/
│       │   │   └── Repositories/
│       │   └── AppDbContextFactory.cs # (Se implementada)
│       ├── Program.cs                # Configuração da Aplicação e Pipeline
│       └── Dockerfile                # Instruções para construir a imagem Docker da API
│
├── tests/
│   └── ManoelsPackaging.Core.UnitTests/ # Projeto de Testes Unitários
│
├── docker-compose.yml              # Orquestração dos contentores API e BD
├── launchSettings.json
├── .dockerignore
├── ManoelsPackagingOptimiser.sln   # Ficheiro da Solução
└── README.md                       # Este ficheiro
```
## 👤 Autor

Matheus Delmondes da Conceição
* LinkedIn: [Matheus Delmondes](https://www.linkedin.com/in/matheus-delmondes-7260b621/)
* Email: [Matheus Delmondes - Outlook](mdelmondes5@outlook.com)
* GitHub: [MatheusDateu](https://github.com/MatheusDateu)

---
