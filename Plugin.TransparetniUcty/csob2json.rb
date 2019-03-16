#!/usr/bin/env ruby

require 'json'
require 'securerandom'

class Entry
  attr_reader :bank_account
  attr_reader :date
  attr_reader :payment
  attr_reader :payer
  attr_reader :row_no
  attr_reader :amount
  attr_reader :account
  attr_reader :vs
  attr_reader :ks
  attr_reader :ss
  attr_reader :note

  def initialize(bank_account, row)
    @bank_account = bank_account
    @row = row
  end
  
  def account_details(account)
    @account_details = account
  end

  def notes(note)
    @notes = note
  end

  def parse
    parse_entry
    parse_account
    parse_notes
  end

  def to_json
    {
      'Id' => SecureRandom.uuid,
      'CisloUctu' => @bank_account,
      'Datum' => @date,
      'PopisTransakce' => @payment,
      'NazevProtiuctu' => @payer,
      'CisloProtiuctu' => @account,
      'ZpravaProPrijemce' => @note,
      'VS' => @vs,
      'KS' => @ks,
      'SS' => @ss,
      'Castka' => @amount,
      'AddId' => @row_no
    }
  end

  private

  def parse_entry
    row = preparse(@row)
    return if row == ''
    # date fix
    parsed = row.gsub(/\A([0-9]{2}\.[0-9]{2}\.)[ ]{2}([^ ])/, '\1;\2').split(/;/)
    i = 0
    @date = parsed[i]
    i += 1
    @payment = parsed[i]
    i += 1
    if parsed.count == 5 
      @payer = parsed[i]
      i += 1
    end
    max = parsed.size
    @row_no = parsed[max-2].to_i
    @amount = heal(parsed[max-1]).to_f
  end

  def parse_account
    return unless @account_details
    @account_details = preparse(@account_details).split(/;/)
    @account = @account_details[0]
    @vs = @account_details[1]
    @ks = @account_details[2]
    @ss = @account_details[3]
  end

  def parse_notes
    @note = @notes
    #@note = preparse(@notes) if @notes
  end

  def preparse(row)
    return unless row
    row.gsub(/\A[ \t]*/, '').gsub(/[ ]{3,}/, ';').chomp
  end

  def heal(string)
    return unless string
    string.gsub(/[^0-9.,-]*/, '').gsub(',', '.')
  end
end

pdf = ARGV[0]

STDERR.puts "Reading #{pdf}"

file = File.readlines(pdf)

data = []
data_entries = []

file.each_with_index do |line, i|
  if !@cislo_uctu && line =~ /\A(Číslo účtu|Účet|Císlo úctu):[ \t]*([0-9-]*\/[0-9]{4})/
    @cislo_uctu = $2
    STDERR.puts "Cislo uctu #{@cislo_uctu}\n"
  end
  next unless line =~ /\A[ ]*[0-9]{2}\.[0-9]{2}\.[ ]{2}/
  data << i
end

STDERR.puts "Found data on lines #{data.map{|a| a+1}.join(',')}."

# processing data
data.each_with_index do |entry_line, i|
  endline = (i<data.size-1) ? data[i+1]-1 : file.size
  block = file[entry_line..endline].reject{|l| l.gsub(/\A[ \t\n]*\Z/,'').empty? }.map{|l| l.gsub(/\A[ \t]*/, '').gsub(/[ ]{2,}/, ';').gsub(/([^;0-9.]);([^;0-9.])/,'\1 \2').chomp}
  STDERR.puts "1: '#{block[0]}'"
  STDERR.puts "2: '#{block[1]}'"
  STDERR.puts "3: '#{block[2]}'"
  STDERR.puts "-" * 80
  data_entry = Entry.new(@cislo_uctu, block[0])
  # no additional data, skip to next
  if block.size > 1
    data_entry.account_details(block[1])
    data_entry.notes(block[2])
  end
  data_entries << data_entry
end
data_entries.map{|a| a.parse}
puts data_entries.map(&:to_json).to_json
