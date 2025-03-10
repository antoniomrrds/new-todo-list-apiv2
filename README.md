# 📦new-todo-list-apiv2

Uma api completa
- sistema de login
- upload de imagem
- refreshtoken
- salvando no cookie 
- cadasto de tag , categorias , todo
## 📋 Pré-requisitos
Antes de começar, certifique-se de ter instalado:

- 🐳 [Docker](https://www.docker.com/get-started)
- ⚙️ [.NET SDK](https://dotnet.microsoft.com/download)
- ⚙️ [Azure Storage Explorer today](https://azure.microsoft.com/en-us/products/storage/storage-explorer)
- 🖥️ [Visual Studio Community 2022](https://visualstudio.microsoft.com/vs/community/) ou sua IDE preferida.

  

## Rodando Localmente 🚀

Para rodar este projeto localmente, siga as instruções abaixo:



1. **Clone o projeto:**

```bash
git clone https://github.com/antoniomrrds/new-todo-list-apiv2.git
```

2. **Entre no diretório do projeto:**

```bash
cd ./new-todo-list-apiv2
```

3. **Inicie o ambiente com Docker Compose: 🐳**

```bash
docker-compose up -d 
```

5. **Abra o projeto no Visual Studio Community 2022 (ou sua IDE preferida):**

   - Abra o Visual Studio.
   - Clique em "Arquivo" > "Abrir" > "Projeto/Solução...".
   - Navegue até a pasta do projeto e selecione o arquivo `Todo-List.sln`.
   - Clique em "Abrir".

6. **Execute o projeto:**

   - No Visual Studio, pressione F5 ou clique em "Depurar" > "Iniciar Depuração".

   Ou

   - No terminal, execute o comando:

   ```bash
   dotnet run
   ```
     O projeto estará acessível em `http://localhost:5277/swagger/index.html` no seu navegador.

 Ou

   - No terminal, execute o comando:

   ```bash
   dotnet watch
   ```

7. **Inicie o Azure Storage Explorer today**

```bash
antes lembre de ter baixado Azure Storage Explorer today
crie um blob chamado images
```
 
## Screenshots 📸
![Captura de tela 2025-03-10 184523](https://github.com/user-attachments/assets/dcae9e8f-b0db-486a-842a-c8d42600b438)

## Stack utilizada 🛠️
**Back-end:** 
- 🖥️ C#
- ⚙️ ASP.NET Core
- 📦 Dapper
- ⚙️ azurite

**DataBase:** 
- 🗄️ MySQL


## 🚀 Sobre mim ℹ️
Eu sou um desenvolvedor backend em ascensão, apaixonado por tecnologia e sempre em busca de aprender coisas novas. Este projeto é um exemplo do meu trabalho e aprendizado contínuo na área de desenvolvimento de software. Se tiver alguma sugestão ou feedback, sinta-se à vontade para entrar em contato! 😊✨

Espero que isso atenda às suas expectativas! Se precisar de mais alguma coisa, estou à disposição.
