using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;


class BasicCommand
{
    protected String _command;
    protected String _description;
    public String command
    {
        get
        {
            return _command;
        }
        private set { }
    }
    public String description
    {
        get
        {
            return _description;
        }
        private set { }
    }
}
class BotCommand<X> : BasicCommand
{
    public delegate String CommandHandler(X arg);
    public CommandHandler handler;

    public BotCommand(string command, string description, CommandHandler handler)
    {
        _command = command;
        _description = description;
        this.handler = handler;
    }

    public String Handle(X arg)
    {
        return handler.Invoke(arg);
    }
}

class INN_Client
{
    private String _token;
    private String _url;
    private HttpClient client;

    public INN_Client(string token, string url)
    {
        _token = token;
        _url = url;
    }

    public void InitClient() 
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(_url);
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<String> GetResponse(String paramName, String value)
    {
        String res = "";
        String query = $"company?key={_token}&{paramName}={value}";
        HttpResponseMessage response = await client.GetAsync(client.BaseAddress + query);
        if (response.IsSuccessStatusCode)
        {
            res = await response.Content.ReadAsStringAsync();
            Company comp = JsonSerializer.Deserialize<Company>(res);
            if (comp.meta.message == null)
            {
                res = comp.getInfo();
            } else
            {
                res = "Организация не найдена";
            }

        }
        return res;
    }
}

public class Company
{
    public CompanyMainData data { get; set; }
    public Meta meta { get; set; }

    public String getInfo()
    {
        return $"НАИМЕНОВАНИЕ: {data?.Name}\nАДРЕС: {data?.LegalAddress?.Address}";
    }
}

public class CompanyMainData
{
    [JsonPropertyName("НаимПолн")]
    public String Name { get; set; }

    [JsonPropertyName("ЮрАдрес")]
    public CompanyLegalAddress LegalAddress { get; set; }
}

public class CompanyLegalAddress
{
    [JsonPropertyName("АдресРФ")]
    public String Address { get; set; }
}

public class Meta
{
    public String status { get; set; }
    public String message { get; set; }
}

public class Configuration
{
    private IConfigurationRoot IConfigRoot;

    public Configuration(String filePath)
    {
        IConfigRoot = new ConfigurationBuilder().AddJsonFile(filePath, false, false).Build();
    }

    public String getConfigValue(string key)
    {
        String? value = "";
        try
        {
            value = IConfigRoot.GetValue<String>(key);
        } 
        catch (Exception ex) { }

        return value;
    }
}


