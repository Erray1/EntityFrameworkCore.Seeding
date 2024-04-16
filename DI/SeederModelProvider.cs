using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.DI;

public sealed class SeederModelProvider
{
    private readonly SeederModelInfo _model;
    public SeederModelProvider(SeederModelInfo model)
    {
        _model = model;
    }
    public SeederModelInfo GetModel() { return _model; }
}
