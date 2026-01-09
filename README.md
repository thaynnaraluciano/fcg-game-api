# FIAP Cloud Games
### Uma plataforma de venda de jogos digitais e gestão de servidores para partidas online.

#### Projeto desenvolvido para o Tech Challenge FIAP, da turma 9NETT. Realizado pelos alunos: 
 - Antonio Renato Pastoriza Ferreira De Souza
 - Bruno Luiz de Souza
 - Thaynnara Luciano dos Santos e
 - Vitor de Oliveira Rennó Ribeiro

# API de Jogos

## Instruções 
Para executar a aplicação em ambiente local, siga os passos a seguir: 
* Faça o clone do repositório para um diretório da sua máquina
    ```
    git clone https://github.com/thaynnaraluciano/fcg-game-api.git
    ```
* Acesse o diretório *fcg-game-api*
    ```
    cd fcg-game-api
    ```
* Abra a solution *FCG.Game.sln* (disponível em: *fcg-game-api*) com o Visual Studio.

## Escolhas de desenvolvimento

A aplicação foi separada em microsserviços, sendo este o microsserviço de jogos.
Esta API foi feita em camadas, sendo elas: Presentation, Domain, Infrastructure, CrossCutting e Tests. Todos os projetos foram desevolvidos seguindo as boas práticas de programação e foram baseadas no DDD.

Os testes criados foram unitários, utilizando as bibliotecas xUnit e Bogus. Os testes foram focados em garantir o funcionamento das validações e o tratamento de exceções.

Para logs foi utilizada a interface ILogger da biblioteca Microsoft.Extensions.Logging. Utilizando-a é possível gerar logs de information, warning, error, entre outros e os logs podem ser consultados no ambiente onde for realizado o deploy.

O tratamento de exceções é realizado pela captura de exceções e tratamento conforme statusCodes do Http via exceptions personalizadas. A captura e retorno padronizado das exceções foi configurado via Middleware.

Foi utilizado Elastic Search para consulta otimizada dos jogos e recomendações específicas para cada usuário.

O microsserviço foi disponibilizado de forma segura através de api gateway.
