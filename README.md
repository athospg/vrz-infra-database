# VRZ Infra Database

Generic database context pattern.

![.NET 5 Tests](https://github.com/athospg/vrz-infra-database/workflows/.NET%205%20Tests/badge.svg)
![Nuget Publish](https://github.com/athospg/vrz-infra-database/workflows/Nuget%20Publish/badge.svg)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

What things you need to install the software and how to install them.

#### SSH access (optional)

```bash
ls -al ~/.ssh
ssh-keygen -t rsa -b 4096 -C your-mail@mail.com
```

### Installing

A step by step series of examples that tell you how to get a development env running

#### NET 6 preview

```bash
mkdir $HOME/dotnet_install && cd $HOME/dotnet_install
curl -H 'Cache-Control: no-cache' -L https://aka.ms/install-dotnet-preview -o install-dotnet-preview.sh
sudo bash install-dotnet-preview.sh
```

\[Optional\] Telemetry opt-out:

```bash
export DOTNET_CLI_TELEMETRY_OPTOUT=1
```

## Update nuget packages

```bash
dotnet list package --outdated --include-prerelease
```

```bash
dotnet add package {name} --version {version}
```

## Running tests

Attach debugger to tests:

```bash
export VSTEST_HOST_DEBUG=1
```

### Coverage

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool

dotnet tool install dotnet-reportgenerator-globaltool --tool-path tools

dotnet new tool-manifest
dotnet tool install dotnet-reportgenerator-globaltool
```

## Deployment

- [Github](https://github.com/features/actions) - Pipelines

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/athospg/vrz-infra-database/tags).

## Authors

- **Athos Póvoa Garcia** - [athospg](https://github.com/athospg)

See also the list of [contributors](https://github.com/athospg/vrz-infra-database/contributors) who participated in this project.

## License

This project is licensed under the GNU Affero General Public License - see the [LICENSE.md](LICENSE.md) file for details

```text
    VRZ Infra Database
    Copyright (C) 2021 Athos Póvoa Garcia

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
```

## Acknowledgments

- Thanks to ...
