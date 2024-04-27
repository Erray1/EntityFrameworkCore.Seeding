using EntityFrameworkCore.Seeding.Core.CreationPolicies;
using EntityFrameworkCore.Seeding.DI;
using EntityFrameworkCore.Seeding.Modelling;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Seeding.Core;
public sealed class SeederEntityCreator
{
    private readonly SeederModelInfo _seederModel;
    private List<SeederEntityInfo>.Enumerator _entitiesEnumerator;

    private ISeederEntityCreationPolicy _currentPolicy;
    private readonly SeederEntityCreaationPolicyFactory _policyFactory;

    private Dictionary<SeederEntityInfo, IEnumerable<object>> _createdEntities = new();
    public SeederEntityCreator(
        SeederModelProvider seederModelProvider,
        SeederEntityCreaationPolicyFactory policyFactory
        ) 
    {
        _seederModel = seederModelProvider.GetModel();
        _entitiesEnumerator = _seederModel.Entities.GetEnumerator();
        _policyFactory = policyFactory;
        _currentPolicy = _policyFactory.CreatePolicyFor(_seederModel.Entities.First());
    }
    public Dictionary<SeederEntityInfo, IEnumerable<object>> CreateEntities()
    {
        createEntitiesInternal();
        return _createdEntities;
    }

    private void createEntitiesInternal()
    {
        var current = _entitiesEnumerator.Current;
        var entities = _currentPolicy.CreateEntities(current);
        _createdEntities.Add(current, entities);
        var isEnd = !_entitiesEnumerator.MoveNext();
        if (isEnd) return;
        _currentPolicy = _policyFactory.CreatePolicyFor(_entitiesEnumerator.Current);
        createEntitiesInternal();
    }
}

