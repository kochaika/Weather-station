<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebApp.About" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
        <h2>Our app description page.</h2>
        <h3>Формулировка задания, над которым производилась работа:</h3>
    </hgroup>

    <article>
        <ul>
            <li>Разработать ПО погодной станции. Система должна состоять из двух модулей:
                <ul>
                    <li>Основной модуль:
                        <ul>
                            <li>Имеет Web-интерфейс управления</li>
                            <li>Отдает информацию по JSON, взаимодействует с соцсетями (по выбору)</li>
                            <li>Синхронизирует время по NTP</li>
                            <li>Отправляет команды/получает данные вспомогательному модулю на частоте 2.4GHz</li>
                        </ul>
                    </li>
                    <li>Датчик – измеряет температуру (и возможно другие параметры), передает информацию основному модулю</li>
                </ul>
                <li>Платформа: Основной модуль: .Net micro, датчик: NXP ARM low-power.</li>
            <li>Сложность: 3</li>
        </ul>
    </article>

    <aside>
        <h3>Aside Title</h3>
        <p>
            Use this area to provide additional information.
        </p>
        <ul>
            <li><a id="A1" runat="server" href="~/">Home</a></li>
            <li><a id="A2" runat="server" href="~/About">About</a></li>
            <li><a id="A3" runat="server" href="~/Contact">Contact</a></li>
        </ul>
    </aside>
</asp:Content>