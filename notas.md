# Notas para o tp

## TODO

- verificar as verficacoes do create do funcionario

## sources

- ASP.NET Core MVC Course (.NET 5)
    - [video](https://www.youtube.com/watch?v=Pi46L7UYP8I)
    - [github](https://github.com/dotnetmastery/Rocky)


## notas da db

guardar alteracoes do identity

`add-migration <nome da migracao>`

aplicar alteracoes

`update-database`

## roles

Sobre as roles de cada user na db só será registado os admins e funcionarios, pois os clientes podem ser considerados todos os que estao logados, e os gerentes sao aqueles que registaram imoveis
- admin
- func

## admin user

User:
- root@root.com
Pass:
- root

## estrutura de dados


- users
    - Id
    - nome
    - role(func, admin)
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
    - avaliacao
    - comentario
    - reservanteId
    - dataInicial
    - dataFinal

- feedback
    - id
    - estrelas
    - comentario
    - userId
    - imovelId

