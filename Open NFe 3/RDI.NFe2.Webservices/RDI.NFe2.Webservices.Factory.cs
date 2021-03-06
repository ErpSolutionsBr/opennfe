﻿using System;
using System.Web.Services;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.ComponentModel;
using RDI.NFe2.SchemaXML;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace RDI.NFe2.Webservices
{
    public static class WSUtils
    {
        private static List<UrlConfiguration> urlsTable = null;

        private static Assembly GetCurrentAssembly()
        {
            return typeof(WSUtils).Assembly;
        }

        public static System.Web.Services.Protocols.SoapHttpClientProtocol SoapHttpClientFactory(TServer ServerName, string Environment, TService ServiceType)
        {
            String ClassName = "";
            try
            {
                var url = GetURL(ServerName, Environment, ServiceType);
                ClassName = $"RDI.NFe2.Webservices.NF4.HUB.{  ServiceType.ToString() }.{GetWSName(ServiceType)}";
                //ClassName = $"RDI.NFe2.Webservices.NF4.HUB.{  ServiceType.ToString() }.{GetWSName(ServiceType)}{((ServerName == TServer.NFe_SVAN || ServerName == TServer.NFe_SVCAN) ? "ForaPadrao" : "")}";
                //ClassName = $"RDI.NFe2.Webservices.NF4.HUB.{  ServiceType.ToString() }.{GetWSName(ServiceType)}{((ServerName == TServer.NFe_SVCAN) ? "ForaPadrao" : "")}";

                Type classType = GetCurrentAssembly().GetType(ClassName);

                if (classType == null)
                    throw new Exception("Não foi possível definir o tipo do cliente de webservice. #ClientProxyFactory");

                var oServico = (System.Web.Services.Protocols.SoapHttpClientProtocol)System.Activator.CreateInstance(classType, url);

                return oServico;
            }
            catch (Exception ex)
            {
                throw new Exception("ClientProxyFactory # não foi possível criar o cliente (" + ClassName + ") para acesso aos webservices da SEFAZ. InnerException: " + ex.Message);
            }
        }

        public static string GetWSMethod(TService ServiceType)
        {
            var hub = GetHubConfiguration(ServiceType);
            return hub.WSMethod;
        }

        private static string GetWSName(TService ServiceType)
        {
            var hub = GetHubConfiguration(ServiceType);
            return hub.WSName;
        }

        private static HubConfiguration GetHubConfiguration(TService ServiceType)
        {
            var hub = (HubConfiguration)ServiceType.GetType().GetField(ServiceType.ToString()).GetCustomAttributes(typeof(HubConfiguration), false).FirstOrDefault();
            if (hub == null)
                throw new Exception("Serviço não está declarado.");
            if (string.IsNullOrEmpty(hub.WSName) || string.IsNullOrEmpty(hub.WSMethod))
                throw new Exception("Serviço não está associado.");

            return hub;
        }

        private static string GetURL(TServer ServerName, string Environment, TService ServiceType)
        {
            CreateUrlTable();

            var urlConfig = urlsTable.FirstOrDefault(p => p.Environment == Environment
                                           && p.ServerName == ServerName
                                           && p.ServiceType == ServiceType);

            if (urlConfig == null)
                throw new Exception("Url não foi definida.");

            return urlConfig.Url;
        }

        private static void CreateUrlTable()
        {
            if (urlsTable != null)
                return;

            urlsTable = new List<UrlConfiguration>()
            {

                #region NFCe Homologacao
                new UrlConfiguration(TServer.NFCe_AM, "Homologacao", TService.Inutilizacao,"https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeInutilizacao4"),
                new UrlConfiguration(TServer.NFCe_AM, "Homologacao", TService.ConsultaProtocolo,"https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeConsulta4"),
                new UrlConfiguration(TServer.NFCe_AM, "Homologacao", TService.Status,"https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeStatusServico4"),
                new UrlConfiguration(TServer.NFCe_AM, "Homologacao", TService.RecepcaoEvento,"https://homnfce.sefaz.am.gov.br/nfce-services/services/RecepcaoEvento4"),
                new UrlConfiguration(TServer.NFCe_AM, "Homologacao", TService.Autorizacao,"https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_AM, "Homologacao", TService.RetAutorizacao,"https://homnfce.sefaz.am.gov.br/nfce-services/services/NfeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFCe_MS, "Homologacao", TService.Inutilizacao,"https://hom.nfce.sefaz.ms.gov.br/ws/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFCe_MS, "Homologacao", TService.ConsultaProtocolo,"https://hom.nfce.sefaz.ms.gov.br/ws/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFCe_MS, "Homologacao", TService.Status,"https://hom.nfce.sefaz.ms.gov.br/ws/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFCe_MS, "Homologacao", TService.RecepcaoEvento,"https://hom.nfce.sefaz.ms.gov.br/ws/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFCe_MS, "Homologacao", TService.Autorizacao,"https://hom.nfce.sefaz.ms.gov.br/ws/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_MS, "Homologacao", TService.RetAutorizacao,"https://hom.nfce.sefaz.ms.gov.br/ws/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFCe_GO, "Homologacao", TService.Autorizacao  ,"https://homolog.sefaz.go.gov.br/nfe/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_GO, "Homologacao", TService.ConsultaProtocolo    ,"https://homolog.sefaz.go.gov.br/nfe/services/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFCe_GO, "Homologacao", TService.Inutilizacao ,"https://homolog.sefaz.go.gov.br/nfe/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFCe_GO, "Homologacao", TService.RetAutorizacao   ,"https://homolog.sefaz.go.gov.br/nfe/services/NFeRetAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_GO, "Homologacao", TService.Status,"https://homolog.sefaz.go.gov.br/nfe/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFCe_GO, "Homologacao", TService.RecepcaoEvento  ,"https://homolog.sefaz.go.gov.br/nfe/services/NFeRecepcaoEvento4"),

                new UrlConfiguration(TServer.NFCe_PR, "Homologacao", TService.Inutilizacao,"https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFCe_PR, "Homologacao", TService.ConsultaProtocolo,"https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFCe_PR, "Homologacao", TService.Status,"https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFCe_PR, "Homologacao", TService.RecepcaoEvento,"https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFCe_PR, "Homologacao", TService.Autorizacao,"https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_PR, "Homologacao", TService.RetAutorizacao,"https://homologacao.nfce.sefa.pr.gov.br/nfce/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFCe_RS, "Homologacao", TService.Inutilizacao,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Homologacao", TService.ConsultaProtocolo,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Homologacao", TService.Status,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Homologacao", TService.RecepcaoEvento,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Homologacao", TService.Autorizacao,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Homologacao", TService.RetAutorizacao,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFCe_SP, "Homologacao", TService.Inutilizacao,"https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeInutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Homologacao", TService.ConsultaProtocolo,"https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeConsultaProtocolo4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Homologacao", TService.Status,"https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Homologacao", TService.RecepcaoEvento,"https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeRecepcaoEvento4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Homologacao", TService.Autorizacao,"https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Homologacao", TService.RetAutorizacao,"https://homologacao.nfce.fazenda.sp.gov.br/ws/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFCe_SVRS, "Homologacao", TService.Inutilizacao,"https://nfce-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Homologacao", TService.ConsultaProtocolo,"https://nfce-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Homologacao", TService.Status,"https://nfce-homologacao.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Homologacao", TService.RecepcaoEvento,"https://nfce-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Homologacao", TService.Autorizacao,"https://nfce-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Homologacao", TService.RetAutorizacao,"https://nfce-homologacao.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),
                #endregion

                #region NFCe Producao
                new UrlConfiguration(TServer.NFCe_AM, "Producao", TService.Inutilizacao,"https://nfce.sefaz.am.gov.br/nfce-services/services/NfeInutilizacao4"),
                new UrlConfiguration(TServer.NFCe_AM, "Producao", TService.ConsultaProtocolo,"https://nfce.sefaz.am.gov.br/nfce-services/services/NfeConsulta4"),
                new UrlConfiguration(TServer.NFCe_AM, "Producao", TService.Status,"https://nfce.sefaz.am.gov.br/nfce-services/services/NfeStatusServico4"),
                new UrlConfiguration(TServer.NFCe_AM, "Producao", TService.RecepcaoEvento,"https://nfce.sefaz.am.gov.br/nfce-services/services/RecepcaoEvento4"),
                new UrlConfiguration(TServer.NFCe_AM, "Producao", TService.Autorizacao,"https://nfce.sefaz.am.gov.br/nfce-services/services/NfeAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_AM, "Producao", TService.RetAutorizacao,"https://nfce.sefaz.am.gov.br/nfce-services/services/NfeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFCe_MS, "Producao", TService.Inutilizacao,"https://nfce.sefaz.ms.gov.br/ws/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFCe_MS, "Producao", TService.ConsultaProtocolo,"https://nfce.sefaz.ms.gov.br/ws/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFCe_MS, "Producao", TService.Status,"https://nfce.sefaz.ms.gov.br/ws/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFCe_MS, "Producao", TService.RecepcaoEvento,"https://nfce.sefaz.ms.gov.br/ws/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFCe_MS, "Producao", TService.Autorizacao,"https://nfce.sefaz.ms.gov.br/ws/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_MS, "Producao", TService.RetAutorizacao,"https://nfce.sefaz.ms.gov.br/ws/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFCe_GO, "Producao", TService.Autorizacao  ,"https://nfe.sefaz.go.gov.br/nfe/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_GO, "Producao", TService.ConsultaProtocolo    ,"https://nfe.sefaz.go.gov.br/nfe/services/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFCe_GO, "Producao", TService.Inutilizacao ,"https://nfe.sefaz.go.gov.br/nfe/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFCe_GO, "Producao", TService.RetAutorizacao   ,"https://nfe.sefaz.go.gov.br/nfe/services/NFeRetAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_GO, "Producao", TService.Status,"https://nfe.sefaz.go.gov.br/nfe/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFCe_GO, "Producao", TService.RecepcaoEvento  ,"https://nfe.sefaz.go.gov.br/nfe/services/NFeRecepcaoEvento4"),

                new UrlConfiguration(TServer.NFCe_PR, "Producao", TService.Inutilizacao,"https://nfce.sefa.pr.gov.br/nfce/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFCe_PR, "Producao", TService.ConsultaProtocolo,"https://nfce.sefa.pr.gov.br/nfce/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFCe_PR, "Producao", TService.Status,"https://nfce.sefa.pr.gov.br/nfce/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFCe_PR, "Producao", TService.RecepcaoEvento,"https://nfce.sefa.pr.gov.br/nfce/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFCe_PR, "Producao", TService.Autorizacao,"https://nfce.sefa.pr.gov.br/nfce/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFCe_PR, "Producao", TService.RetAutorizacao,"https://nfce.sefa.pr.gov.br/nfce/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFCe_RS, "Producao", TService.Inutilizacao ,"https://nfe.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Producao", TService.ConsultaProtocolo    ,"https://nfe.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Producao", TService.Status    ,"https://nfe.sefazrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Producao", TService.RecepcaoEvento  ,"https://nfe.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Producao", TService.Autorizacao  ,"https://nfe.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_RS, "Producao", TService.RetAutorizacao   ,"https://nfe.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFCe_SP, "Producao", TService.Inutilizacao,"https://nfce.fazenda.sp.gov.br/ws/NFeInutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Producao", TService.ConsultaProtocolo,"https://nfce.fazenda.sp.gov.br/ws/NFeConsultaProtocolo4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Producao", TService.Status,"https://nfce.fazenda.sp.gov.br/ws/NFeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Producao", TService.RecepcaoEvento,"https://nfce.fazenda.sp.gov.br/ws/NFeRecepcaoEvento4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Producao", TService.Autorizacao,"https://nfce.fazenda.sp.gov.br/ws/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_SP, "Producao", TService.RetAutorizacao,"https://nfce.fazenda.sp.gov.br/ws/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFCe_SVRS, "Producao", TService.Inutilizacao,"https://nfce.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Producao", TService.ConsultaProtocolo,"https://nfce.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Producao", TService.Status,"https://nfce.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Producao", TService.RecepcaoEvento,"https://nfce.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Producao", TService.Autorizacao,"https://nfce.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFCe_SVRS, "Producao", TService.RetAutorizacao,"https://nfce.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),
                #endregion


                #region NFE homologacao
                new UrlConfiguration(TServer.NFe_AM, "Homologacao", TService.Inutilizacao,"https://homnfe.sefaz.am.gov.br/services2/services/NfeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_AM, "Homologacao", TService.ConsultaProtocolo,"https://homnfe.sefaz.am.gov.br/services2/services/NfeConsulta4"),
                new UrlConfiguration(TServer.NFe_AM, "Homologacao", TService.Status,"https://homnfe.sefaz.am.gov.br/services2/services/NfeStatusServico4"),
                new UrlConfiguration(TServer.NFe_AM, "Homologacao", TService.RecepcaoEvento,"https://homnfe.sefaz.am.gov.br/services2/services/RecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_AM, "Homologacao", TService.Autorizacao,"https://homnfe.sefaz.am.gov.br/services2/services/NfeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_AM, "Homologacao", TService.RetAutorizacao,"https://homnfe.sefaz.am.gov.br/services2/services/NfeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_BA, "Homologacao", TService.RecepcaoEvento,"https://hnfe.sefaz.ba.gov.br/webservices/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Homologacao", TService.Inutilizacao,"https://hnfe.sefaz.ba.gov.br/webservices/NFeInutilizacao4/NFeInutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Homologacao", TService.ConsultaProtocolo,"https://hnfe.sefaz.ba.gov.br/webservices/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Homologacao", TService.Status,"https://hnfe.sefaz.ba.gov.br/webservices/NFeStatusServico4/NFeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Homologacao", TService.Autorizacao,"https://hnfe.sefaz.ba.gov.br/webservices/NFeAutorizacao4/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Homologacao", TService.RetAutorizacao,"https://hnfe.sefaz.ba.gov.br/webservices/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_CE, "Homologacao", TService.RecepcaoEvento,"https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_CE, "Homologacao", TService.Inutilizacao,"https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_CE, "Homologacao", TService.ConsultaProtocolo,"https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_CE, "Homologacao", TService.Status,"https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_CE, "Homologacao", TService.Autorizacao,"https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_CE, "Homologacao", TService.RetAutorizacao,"https://nfeh.sefaz.ce.gov.br/nfe4/services/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_GO, "Homologacao", TService.Inutilizacao,"https://homolog.sefaz.go.gov.br/nfe/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_GO, "Homologacao", TService.ConsultaProtocolo,"https://homolog.sefaz.go.gov.br/nfe/services/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_GO, "Homologacao", TService.Status,"https://homolog.sefaz.go.gov.br/nfe/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_GO, "Homologacao", TService.RecepcaoEvento,"https://homolog.sefaz.go.gov.br/nfe/services/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_GO, "Homologacao", TService.Autorizacao,"https://homolog.sefaz.go.gov.br/nfe/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_GO, "Homologacao", TService.RetAutorizacao,"https://homolog.sefaz.go.gov.br/nfe/services/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_MG, "Homologacao", TService.Inutilizacao,"https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_MG, "Homologacao", TService.ConsultaProtocolo,"https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeConsulta4"),
                new UrlConfiguration(TServer.NFe_MG, "Homologacao", TService.Status,"https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_MG, "Homologacao", TService.Autorizacao,"https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_MG, "Homologacao", TService.RetAutorizacao,"https://hnfe.fazenda.mg.gov.br/nfe2/services/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_MS, "Homologacao", TService.Inutilizacao,"https://homologacao.nfe.ms.gov.br/ws/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_MS, "Homologacao", TService.ConsultaProtocolo,"https://homologacao.nfe.ms.gov.br/ws/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_MS, "Homologacao", TService.Status,"https://homologacao.nfe.ms.gov.br/ws/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_MS, "Homologacao", TService.RecepcaoEvento,"https://homologacao.nfe.ms.gov.br/ws/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_MS, "Homologacao", TService.Autorizacao,"https://homologacao.nfe.ms.gov.br/ws/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_MS, "Homologacao", TService.RetAutorizacao,"https://homologacao.nfe.ms.gov.br/ws/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_MT, "Homologacao", TService.Inutilizacao,"https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_MT, "Homologacao", TService.ConsultaProtocolo,"https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeConsulta4"),
                new UrlConfiguration(TServer.NFe_MT, "Homologacao", TService.Status,"https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeStatusServico4"),
                new UrlConfiguration(TServer.NFe_MT, "Homologacao", TService.RecepcaoEvento,"https://homologacao.sefaz.mt.gov.br/nfews/v2/services/RecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_MT, "Homologacao", TService.Autorizacao,"https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_MT, "Homologacao", TService.RetAutorizacao,"https://homologacao.sefaz.mt.gov.br/nfews/v2/services/NfeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_PE, "Homologacao", TService.Inutilizacao ,"https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_PE, "Homologacao", TService.ConsultaProtocolo    ,"https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_PE, "Homologacao", TService.Status, "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_PE, "Homologacao", TService.RecepcaoEvento  ,"https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_PE, "Homologacao", TService.Autorizacao  ,"https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_PE, "Homologacao", TService.RetAutorizacao   ,"https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_PR, "Homologacao", TService.Inutilizacao,"https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_PR, "Homologacao", TService.ConsultaProtocolo,"https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_PR, "Homologacao", TService.Status,"https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_PR, "Homologacao", TService.RecepcaoEvento,"https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_PR, "Homologacao", TService.Autorizacao,"https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_PR, "Homologacao", TService.RetAutorizacao,"https://homologacao.nfe.sefa.pr.gov.br/nfe/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_RS, "Homologacao", TService.Inutilizacao,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Homologacao", TService.ConsultaProtocolo,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Homologacao", TService.Status,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Homologacao", TService.RecepcaoEvento,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Homologacao", TService.Autorizacao,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Homologacao", TService.RetAutorizacao,"https://nfe-homologacao.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SP, "Homologacao", TService.Inutilizacao,"https://homologacao.nfe.fazenda.sp.gov.br/ws/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Homologacao", TService.ConsultaProtocolo,"https://homologacao.nfe.fazenda.sp.gov.br/ws/nfeconsultaprotocolo4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Homologacao", TService.Status,"https://homologacao.nfe.fazenda.sp.gov.br/ws/nfestatusservico4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Homologacao", TService.RecepcaoEvento,"https://homologacao.nfe.fazenda.sp.gov.br/ws/nferecepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Homologacao", TService.Autorizacao,"https://homologacao.nfe.fazenda.sp.gov.br/ws/nfeautorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Homologacao", TService.RetAutorizacao,"https://homologacao.nfe.fazenda.sp.gov.br/ws/nferetautorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SVAN, "Homologacao", TService.Inutilizacao,"https://hom.sefazvirtual.fazenda.gov.br/NFeInutilizacao4/NFeInutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Homologacao", TService.ConsultaProtocolo,"https://hom.sefazvirtual.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Homologacao", TService.Status,"https://hom.sefazvirtual.fazenda.gov.br/NFeStatusServico4/NFeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Homologacao", TService.RecepcaoEvento,"https://hom.sefazvirtual.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Homologacao", TService.Autorizacao,"https://hom.sefazvirtual.fazenda.gov.br/NFeAutorizacao4/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Homologacao", TService.RetAutorizacao,"https://hom.sefazvirtual.fazenda.gov.br/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SVRS, "Homologacao", TService.Inutilizacao,"https://nfe-homologacao.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Homologacao", TService.ConsultaProtocolo,"https://nfe-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Homologacao", TService.Status,"https://nfe-homologacao.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Homologacao", TService.RecepcaoEvento,"https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Homologacao", TService.Autorizacao,"https://nfe-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Homologacao", TService.RetAutorizacao,"https://nfe-homologacao.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SVCAN, "Homologacao", TService.ConsultaProtocolo,"https://hom.svc.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCAN, "Homologacao", TService.Status,"https://hom.svc.fazenda.gov.br/NFeStatusServico4/NFeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCAN, "Homologacao", TService.RecepcaoEvento,"https://hom.svc.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCAN, "Homologacao", TService.Autorizacao,"https://hom.svc.fazenda.gov.br/NFeAutorizacao4/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCAN, "Homologacao", TService.RetAutorizacao,"https://hom.svc.fazenda.gov.br/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SVCRS, "Homologacao", TService.ConsultaProtocolo   ,"https://nfe-homologacao.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCRS, "Homologacao", TService.Status ,"https://nfe-homologacao.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCRS, "Homologacao", TService.RecepcaoEvento  ,"https://nfe-homologacao.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCRS, "Homologacao", TService.Autorizacao  ,"https://nfe-homologacao.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCRS, "Homologacao", TService.RetAutorizacao   ,"https://nfe-homologacao.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),
                #endregion
                
                #region NFE Producao


                new UrlConfiguration(TServer.NFe_AM, "Producao", TService.Inutilizacao ,"https://nfe.sefaz.am.gov.br/services2/services/NfeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_AM, "Producao", TService.ConsultaProtocolo, "https://nfe.sefaz.am.gov.br/services2/services/NfeConsulta4"),
                new UrlConfiguration(TServer.NFe_AM, "Producao", TService.Status, "https://nfe.sefaz.am.gov.br/services2/services/NfeStatusServico4"),
                new UrlConfiguration(TServer.NFe_AM, "Producao", TService.RecepcaoEvento, "https://nfe.sefaz.am.gov.br/services2/services/RecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_AM, "Producao", TService.Autorizacao, "https://nfe.sefaz.am.gov.br/services2/services/NfeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_AM, "Producao", TService.RetAutorizacao, "https://nfe.sefaz.am.gov.br/services2/services/NfeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_BA, "Producao", TService.Inutilizacao, "https://nfe.sefaz.ba.gov.br/webservices/NFeInutilizacao4/NFeInutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Producao", TService.ConsultaProtocolo, "https://nfe.sefaz.ba.gov.br/webservices/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Producao", TService.Status, "https://nfe.sefaz.ba.gov.br/webservices/NFeStatusServico4/NFeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Producao", TService.RecepcaoEvento, "https://nfe.sefaz.ba.gov.br/webservices/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Producao", TService.Autorizacao, "https://nfe.sefaz.ba.gov.br/webservices/NFeAutorizacao4/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_BA, "Producao", TService.RetAutorizacao, "https://nfe.sefaz.ba.gov.br/webservices/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_CE, "Producao", TService.Inutilizacao ,"https://nfe.sefaz.ce.gov.br/nfe4/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_CE, "Producao", TService.ConsultaProtocolo,"https://nfe.sefaz.ce.gov.br/nfe4/services/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_CE, "Producao", TService.Status ,"https://nfe.sefaz.ce.gov.br/nfe4/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_CE, "Producao", TService.RecepcaoEvento  ,"https://nfe.sefaz.ce.gov.br/nfe4/services/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_CE, "Producao", TService.Autorizacao  ,"https://nfe.sefaz.ce.gov.br/nfe4/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_CE, "Producao", TService.RetAutorizacao  ,"https://nfe.sefaz.ce.gov.br/nfe4/services/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_GO, "Producao", TService.Inutilizacao, "https://nfe.sefaz.go.gov.br/nfe/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_GO, "Producao", TService.ConsultaProtocolo, "https://nfe.sefaz.go.gov.br/nfe/services/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_GO, "Producao", TService.Status, "https://nfe.sefaz.go.gov.br/nfe/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_GO, "Producao", TService.RecepcaoEvento, "https://nfe.sefaz.go.gov.br/nfe/services/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_GO, "Producao", TService.Autorizacao, "https://nfe.sefaz.go.gov.br/nfe/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_GO, "Producao", TService.RetAutorizacao, "https://nfe.sefaz.go.gov.br/nfe/services/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_MG, "Producao", TService.Inutilizacao ,"https://nfe.fazenda.mg.gov.br/nfe2/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_MG, "Producao", TService.ConsultaProtocolo    ,"https://nfe.fazenda.mg.gov.br/nfe2/services/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_MG, "Producao", TService.Status, "https://nfe.fazenda.mg.gov.br/nfe2/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_MG, "Producao", TService.RecepcaoEvento  ,"https://nfe.fazenda.mg.gov.br/nfe2/services/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_MG, "Producao", TService.Autorizacao  ,"https://nfe.fazenda.mg.gov.br/nfe2/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_MG, "Producao", TService.RetAutorizacao   ,"https://nfe.fazenda.mg.gov.br/nfe2/services/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_MS, "Producao", TService.Inutilizacao ,"https://nfe.fazenda.ms.gov.br/ws/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_MS, "Producao", TService.ConsultaProtocolo    ,"https://nfe.fazenda.ms.gov.br/ws/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_MS, "Producao", TService.Status, "https://nfe.fazenda.ms.gov.br/ws/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_MS, "Producao", TService.RecepcaoEvento  ,"https://nfe.fazenda.ms.gov.br/ws/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_MS, "Producao", TService.Autorizacao  ,"https://nfe.fazenda.ms.gov.br/ws/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_MS, "Producao", TService.RetAutorizacao   ,"https://nfe.fazenda.ms.gov.br/ws/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_MT, "Producao", TService.Inutilizacao, "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_MT, "Producao", TService.ConsultaProtocolo, "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeConsulta4"),
                new UrlConfiguration(TServer.NFe_MT, "Producao", TService.Status, "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeStatusServico4"),
                new UrlConfiguration(TServer.NFe_MT, "Producao", TService.RecepcaoEvento, "https://nfe.sefaz.mt.gov.br/nfews/v2/services/RecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_MT, "Producao", TService.Autorizacao, "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_MT, "Producao", TService.RetAutorizacao, "https://nfe.sefaz.mt.gov.br/nfews/v2/services/NfeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_PE, "Producao", TService.Inutilizacao ,"https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_PE, "Producao", TService.ConsultaProtocolo    ,"https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_PE, "Producao", TService.Status    ,"https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_PE, "Producao", TService.RecepcaoEvento  ,"https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_PE, "Producao", TService.Autorizacao  ,"https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_PE, "Producao", TService.RetAutorizacao   ,"https://nfe.sefaz.pe.gov.br/nfe-service/services/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_PR, "Producao", TService.Inutilizacao, "https://nfe.sefa.pr.gov.br/nfe/NFeInutilizacao4"),
                new UrlConfiguration(TServer.NFe_PR, "Producao", TService.ConsultaProtocolo, "https://nfe.sefa.pr.gov.br/nfe/NFeConsultaProtocolo4"),
                new UrlConfiguration(TServer.NFe_PR, "Producao", TService.Status, "https://nfe.sefa.pr.gov.br/nfe/NFeStatusServico4"),
                new UrlConfiguration(TServer.NFe_PR, "Producao", TService.RecepcaoEvento, "https://nfe.sefa.pr.gov.br/nfe/NFeRecepcaoEvento4"),
                new UrlConfiguration(TServer.NFe_PR, "Producao", TService.Autorizacao, "https://nfe.sefa.pr.gov.br/nfe/NFeAutorizacao4"),
                new UrlConfiguration(TServer.NFe_PR, "Producao", TService.RetAutorizacao, "https://nfe.sefa.pr.gov.br/nfe/NFeRetAutorizacao4"),

                new UrlConfiguration(TServer.NFe_RS, "Producao", TService.Inutilizacao ,"https://nfe.sefazrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Producao", TService.ConsultaProtocolo    ,"https://nfe.sefazrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Producao", TService.Status    ,"https://nfe.sefazrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Producao", TService.RecepcaoEvento  ,"https://nfe.sefazrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Producao", TService.Autorizacao  ,"https://nfe.sefazrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_RS, "Producao", TService.RetAutorizacao   ,"https://nfe.sefazrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SP, "Producao", TService.Inutilizacao, "https://nfe.fazenda.sp.gov.br/ws/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Producao", TService.ConsultaProtocolo, "https://nfe.fazenda.sp.gov.br/ws/nfeconsultaprotocolo4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Producao", TService.Status, "https://nfe.fazenda.sp.gov.br/ws/nfestatusservico4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Producao", TService.RecepcaoEvento, "https://nfe.fazenda.sp.gov.br/ws/nferecepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Producao", TService.Autorizacao, "https://nfe.fazenda.sp.gov.br/ws/nfeautorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SP, "Producao", TService.RetAutorizacao, "https://nfe.fazenda.sp.gov.br/ws/nferetautorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SVAN, "Producao", TService.Inutilizacao, "https://www.sefazvirtual.fazenda.gov.br/NFeInutilizacao4/NFeInutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Producao", TService.ConsultaProtocolo, "https://www.sefazvirtual.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Producao", TService.Status, "https://www.sefazvirtual.fazenda.gov.br/NFeStatusServico4/NFeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Producao", TService.RecepcaoEvento, "https://www.sefazvirtual.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Producao", TService.Autorizacao, "https://www.sefazvirtual.fazenda.gov.br/NFeAutorizacao4/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVAN, "Producao", TService.RetAutorizacao, "https://www.sefazvirtual.fazenda.gov.br/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SVRS, "Producao", TService.Inutilizacao ,"https://nfe.svrs.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Producao", TService.ConsultaProtocolo    ,"https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Producao", TService.Status    ,"https://nfe.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Producao", TService.RecepcaoEvento  ,"https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Producao", TService.Autorizacao  ,"https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVRS, "Producao", TService.RetAutorizacao   ,"https://nfe.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SVCAN, "Producao", TService.ConsultaProtocolo, "https://www.svc.fazenda.gov.br/NFeConsultaProtocolo4/NFeConsultaProtocolo4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCAN, "Producao", TService.Status, "https://www.svc.fazenda.gov.br/NFeStatusServico4/NFeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCAN, "Producao", TService.RecepcaoEvento, "https://www.svc.fazenda.gov.br/NFeRecepcaoEvento4/NFeRecepcaoEvento4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCAN, "Producao", TService.Autorizacao, "https://www.svc.fazenda.gov.br/NFeAutorizacao4/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCAN, "Producao", TService.RetAutorizacao, "https://www.svc.fazenda.gov.br/NFeRetAutorizacao4/NFeRetAutorizacao4.asmx"),

                new UrlConfiguration(TServer.NFe_SVCRS, "Producao", TService.ConsultaProtocolo    ,"https://nfe.svrs.rs.gov.br/ws/NfeConsulta/NfeConsulta4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCRS, "Producao", TService.Status    ,"https://nfe.svrs.rs.gov.br/ws/NfeStatusServico/NfeStatusServico4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCRS, "Producao", TService.RecepcaoEvento  ,"https://nfe.svrs.rs.gov.br/ws/recepcaoevento/recepcaoevento4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCRS, "Producao", TService.Autorizacao  ,"https://nfe.svrs.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao4.asmx"),
                new UrlConfiguration(TServer.NFe_SVCRS, "Producao", TService.RetAutorizacao   ,"https://nfe.svrs.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao4.asmx"),
                #endregion

            };
        }
    }
}