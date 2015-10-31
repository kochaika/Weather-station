<%@ Page Title="MDP" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApp._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Welcome to Weather statition</h2>
            </hgroup>
            <p>
                To learn more about our team's project, visit <a href="https://drive.google.com/folderview?id=0B1qJYO_YS5DZU0lKNFZYdDlDTms&usp=sharing_eid&ts=561d3223" title="Our project description on google drive">drive google</a>.  
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h3>Мы предлагаем следущие возможности:</h3>
    <ul>
        <li class="one">
            <h5>Получение погодных данных в текущем времени...</h5>
            ....здесь картинка с погодными данными....
            <ul>
                <li class="two">
                    <h6>Температура</h6>
                    ....здесь данные о температуре....
                </li>
                <li class="three">
                    <h6>Освещенность</h6>
                    ....здесь данные об освещенности....
                </li>
            </ul>
        </li>
    </ul>
</asp:Content>
