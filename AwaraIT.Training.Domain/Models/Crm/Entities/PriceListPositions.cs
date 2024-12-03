using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.Training.Domain.Models.Crm.Entities
{

    [EntityLogicalName(EntityLogicalName)]
    public class PriceListPositions : BaseEntity
    {
        public PriceListPositions() : base(EntityLogicalName) { }
        public static class Metadata
        {
            public const string PriceListPostionId = "fnt_price_list_positionid";
            public const string PriceListReference = "fnt_price_list";
            public const string TerritoryReference = "fnt_territoryid";
            public const string SubjectReference = "fnt_subject";

            public const string FormatPreparationReference = "fnt_format_preparation";
            public const string FormatConductionReference = "fnt_format_condaction";
            public const string Price = "fnt_price";

        }
        public const string EntityLogicalName = "fnt_price_list_position";

        public Guid PriceListPostionId
        {
            get { return GetAttributeValue<Guid>(Metadata.PriceListPostionId); }
            set { Attributes[Metadata.PriceListPostionId] = value; }
        }
        public EntityReference PriceListReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.PriceListReference); }
            set { Attributes[Metadata.PriceListReference] = value; }
        } 
        public EntityReference TerritoryReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.TerritoryReference); }
            set { Attributes[Metadata.TerritoryReference] = value; }
        }
        public EntityReference SubjectReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.SubjectReference); }
            set { Attributes[Metadata.SubjectReference] = value; }
        }
        public EntityReference FormatPreparationReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.FormatPreparationReference); }
            set { Attributes[Metadata.FormatPreparationReference] = value; }
        }
        public EntityReference FormatConductionReference
        {
            get { return GetAttributeValue<EntityReference>(Metadata.FormatConductionReference); }
            set { Attributes[Metadata.FormatConductionReference] = value; }
        }
        public Money Price
        {
            get { return GetAttributeValue<Money>(Metadata.Price); }
            set { Attributes[Metadata.Price] = value; }
        }
    }
}
