using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    //crindo uma documentação para explicar cada método no swagger
    ///<sumary>
    ///    Adiciona um filme ao banco de dados
    ///</sumary>
    ///<param name="filmeDto"> Objeto com os campos necessários para criação de um filme </param>
    ///<returns>IActionResult</returns>
    ///<response code="201"> Caso inserção seja feita com sucesso </response>

    //Aqui vem do post do swagger
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
                                  //do body da aplicação
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto )
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperaFilmeId), new { id = filme.Id },
            filme);
    }

    ///<sumary>
    ///    Consulta de todos os filme no banco de dados
    ///</sumary>
    ///<response code="201"> Caso a pesquisa seja feita com sucesso </response>
    [HttpGet]
           //caso a lista venha mudar, não precisar mudar o cabeçalho aqui do método
    public IEnumerable<ReadFilmeDto> RecuperaFilme([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));
    }

    ///<sumary>
    ///    Consulta de um filme específico no banco de dados
    ///</sumary>
    ///<param name="id"> Objeto com os campos necessários para consultar um filme </param>
    ///<returns>IActionResult</returns>
    ///<response code="201"> Caso a pesquisa seja feita com sucesso </response>
    [HttpGet("{id}")]
    public IActionResult RecuperaFilmeId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

        if (filme == null) return NotFound();
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto);
    }

    ///<sumary>
    ///    Atualizar filme no banco de dados
    ///</sumary>
    ///<param name="filmeDto"> Objeto com os campos necessários para a atualização de um filme </param>
    ///<returns>IActionResult</returns>
    ///<response code="201"> Caso a atualização seja feita com sucesso </response>
    [HttpPut("{id}")]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);
        if(filme == null) return NotFound();
        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();
        return NoContent();
    }

    ///<sumary>
    ///    Atualização do filme sem utilizar todos os campos banco de dados
    ///</sumary>
    ///<param name="id"> Objeto com os campos necessários para atualização de um filme </param>
    ///<returns>IActionResult</returns>
    ///<response code="201"> Caso a atualização seja feita com sucesso </response>
    [HttpPatch("{id}")]
    public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);
        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if (!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }
        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();
        return NoContent();
    }
    ///<sumary>
    ///    Deletar filme no banco de dados
    ///</sumary>
    ///<param name="id"> Objeto com os campos necessários para deletar um filme </param>
    ///<returns>IActionResult</returns>
    ///<response code="201"> Caso o delete seja feito com sucesso </response>
    [HttpDelete("{id}")]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(
           filme => filme.Id == id);
        if (filme == null) return NotFound();
        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}
