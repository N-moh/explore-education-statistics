# Explore education statistics service
[![Build Status](https://dfe-gov-uk.visualstudio.com/s101-Explore-Education-Statistics/_apis/build/status/Explore%20Education%20Statistics?branchName=master)](https://dfe-gov-uk.visualstudio.com/s101-Explore-Education-Statistics/_build/latest?definitionId=200&branchName=master)

## Getting started

### Requirements

To run the application you require:
- Docker / Docker compose 

To develop the application you will require:
- v9.0.0+ of Node.Js installed
- Version 2.2 of the dotnet SDK installed

### Running the frontend

1. Run the following from the project root to install all project dependencies:

    ```bash
    npm ci
    npm run bootstrap
    ```

2. Startup required any required backend services:
    
    ```bash
    cd src/
    docker-compose up -d data-api content-api
    ```
       
3. Run the frontend applications by running from the project root

    ```bash
    # Admin frontend
    npm run start:admin
    # Public frontend
    npm run start:frontend
    ```

    Alternatively, go into one of the sub-project directories and start them directly:
    
    ```bash
    cd src/explore-education-statistics-frontend
    npm start
    ```

4. Access frontend applications at:

    - `http://localhost:3000` for the public frontend
    - `http://localhost:3001` for the admin frontend

## Frontend development

### Environment variables

Out of the box, each sub-project should come with a default set of environment variables (in `.env`) 
that should work for development.

If you need to change any environment variables only for your local environment, you can create a
corresponding `.env.local` file which will be loaded in preference to `.env`. `.env.local` is 
ignored by Git.

Various `.env.{env}` files are checked into Git for use in our deployment environments, but will not
generally be used for local development.

#### Adding variables

Required environment variables should be supplied to both the specific `.env.{env}` file and the 
`.env.example` file (example versions of variables should be placed here). 

The `.env.example` file is used to validate that the `.env.{env}` file in use is not missing any 
required variables and consequently needs to be in sync with any changes.

### Dependency management with Lerna

The project currently uses [Lerna](https://github.com/lerna/lerna) to handle dependencies as we have
adopted a monorepo project structure and have dependencies between sub-projects. These dependencies
are established using symlinks that Lerna creates.

- `explore-education-statistics-admin` 
    - Contains the admin frontend application. 
    - Single page application based on Create React App.
  
- `explore-education-statistics-frontend` 
    - Contains the public frontend application.
    - This is a server side rendered Next application.
     
- `explore-education-statistics-common` 
    - Contains common code between the other sub-projects for re-use.

#### Adding dependencies

**DO NOT** install (`npm install`) any dependencies directly into the sub-projects as this will 
most likely break the sub-project's `package-lock.json` and cause your installation to fail.

Instead you will need to use Lerna itself to do this, for example:

```bash
# Using global Lerna install
lerna add your-dep src/explore-education-statistics-frontend

# Or, using local Lerna install
./node_modules/.bin/lerna add your-dep src/explore-education-statistics-frontend
```

#### Cleaning dependencies

During development you might end up in an inconsistent state where your sub-project `node_modules` 
are broken for whatever reason. Consequently, it is advisable to clean down your sub-project 
`node_modules` by running the following from the project root.

```bash
npm run clean
```

### Common NPM scripts

These scripts can generally be run from most `package.json` files across the project.

- `npm test` - Run all tests.

- `npm run tsc` - Run Typescript compiler to check types are correct. Does not build anything.

- `npm run lint` - Lint projects using Stylelint and ESLint.
    - `npm run lint:js` - Run ESLint only.
    - `npm run lint:style` - Run Stylelint only.
    
- `npm run fix` - Fix any lint that can be automatically fixed by the linters.
    - `npm run fix:js` - Fix only ESLint lints.
    - `npm run fix:style` - Fix only Stylelint lints.

- `npm run format` - Format codebase using Prettier. 

#### Project root scripts

These can only be run from the project root `package.json`.

- `npm run bootstrap` - Install NPM dependencies across entire project and symlink any dependent 
  modules (only from project root).

- `npm run clean` - Remove any `node_modules` directories across any sub-projects.

- `npm run start:admin` - Run admin frontend dev server.
- `npm run start:frontend` - Run public frontend dev server.
  
#### Sub-project scripts

These can only be run from a sub-project `package.json`.

- `npm start` - Start a sub-project dev server.

### Code style

We use ESLint with the [Airbnb style guide](https://github.com/airbnb/javascript) for the project
code style.

#### Disabling linting upon save

During development, code is linted upon save, but if required this can be disabled by adding the 
following in your `.env.local`:

```
ESLINT_DISABLE=true
```

## Backend development

### Migrations

The backend c# projects use code first migrations to generate the application's database schema.
The entity framework tool will need to be installed as follows:
```
dotnet tool install -g dotnet-ef --version 3.0.0`
```

#### Content DB migrations
To generate a migration for the content db:
```
cd explore-education-statistics\src\GovUk.Education.ExploreEducationStatistics.Admin
dotnet ef migrations add AdjustFiltersIdsPreSeed --context ContentDbContext --output-dir Migrations/ContentMigrations
```
#### Statistics DB migrations
To generate a migration for the statistics db:
```
cd explore-education-statistics\src\GovUk.Education.ExploreEducationStatistics.Data.Api
dotnet ef migrations add Ees183Test --project ..\GovUk.Education.ExploreEducationStatistics.Data.Model
```

## Contributing


## License
This application is licensed under the MIT License.
