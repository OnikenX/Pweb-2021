# Notas para o tp

## TODO

- turnar alguns metodos async


## sources

- ASP.NET Core MVC Course (.NET 5)
    - [video](https://www.youtube.com/watch?v=Pi46L7UYP8I)
    - [github](https://github.com/dotnetmastery/Rocky)


## notas da db

guardar alteracoes do identity

`add-migration <nome da migracao>`

aplicar alteracoes

`update-database`


## estrutura de dados


- users
    - Id
    - nome
    - role(cliente, gestor, funcionario, admin)
    - data de criacao

- imoveis
    - Id
    - donoUserId
    - descricao

- ImovelImagem
    - Id
    - imovelId
    - imagemPath

- reserva
    - Id
    - imovelId
    - reservanteId
    - dataInicial
    - dataFinal
